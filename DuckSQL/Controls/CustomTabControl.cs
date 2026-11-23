using DuckSQL.Analytics;
using DuckSQL.Exceptions;
using DuckSQL.Helpers;
using System.ComponentModel;
using System.Diagnostics;

namespace DuckSQL.Controls
{
    internal class CustomTabControl : TabControl
    {
        private int? _closeButtonHoveredTabIndex = null;
        private Rectangle? _closeButtonHoveredTabRectangle = null;
        private bool _isMouseLeftButtonDown = false;
        private int _rightClickedTabIndex = -1;
        private static int _newQueryCount = 0;

        #region Context Menu
        private readonly ContextMenuStrip _rightClickTabContextMenuStrip = new();
        private readonly ToolStripMenuItem _closeTabToolStripMenuItem = new() { Text = Resources.Strings.CloseTabText };
        private readonly ToolStripMenuItem _closeOtherTabsToolStripMenuItem = new() { Text = Resources.Strings.CloseOtherTabsText };
        private readonly ToolStripMenuItem _closeAllTabsToolStripMenuItem = new() { Text = Resources.Strings.CloseAllTabsText };
        private readonly ToolStripSeparator _firstToolStripSeparator = new();
        private readonly ToolStripMenuItem _openQueryLocationToolStripMenuItem = new() { Text = Resources.Strings.OpenQueryLocationText };
        #endregion

        #region Drag & Drop
        private int? _dragStartTabIndex = null;
        private int? _dragStopTabIndex = null;
        private Point _dragStartLocation = Point.Empty;
        #endregion

        #region Events
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public EventHandler? QueryExecutionStarted { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public EventHandler<QueryEditorControl.QueryExecutedEventArgs>? QueryExecuted { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public EventHandler? TabDirtyStateChanged { get; set; }
        #endregion

        #region Session Caching
        private System.Windows.Forms.Timer _cacheOpenTabsTimer = new() { Enabled = true, Interval = 1000 };
        private readonly SessionCache _sessionCache = new();
        #endregion

        public CustomTabControl()
        {
            DrawMode = TabDrawMode.OwnerDrawFixed;
            _rightClickTabContextMenuStrip.Items.AddRange(
                [_closeTabToolStripMenuItem, _closeOtherTabsToolStripMenuItem, _closeAllTabsToolStripMenuItem,
                _firstToolStripSeparator, _openQueryLocationToolStripMenuItem]);
            _closeTabToolStripMenuItem.Click += closeTabToolStripMenuItem_Click;
            _closeOtherTabsToolStripMenuItem.Click += closeOtherTabsToolStripMenuItem_Click;
            _closeAllTabsToolStripMenuItem.Click += closeAllTabsToolStripMenuItem_Click;
            _openQueryLocationToolStripMenuItem.Click += openQueryLocationToolStripMenuItem_Click;
            AllowDrop = true;
            ShowToolTips = true;
            _cacheOpenTabsTimer.Tick += _cacheOpenTabsTimer_Tick;
        }

        public void ResumeLastSession()
        {
            try
            {
                if (TabPages.Count != 1)
                    throw new InvalidOperationException("Session can only be resumed on startup");

                var session = _sessionCache.Load();
                if (session is null)
                    return; //nothing to restore

                foreach (var tab in session.Tabs)
                {
                    AddTab(tab);
                }

                if (session.SelectedTab is not null)
                    SelectedTab = session.SelectedTab;

                _newQueryCount = session.NewQueryCount;

                TabPages.RemoveAt(0); //Remove the default empty query tab
            }
            catch (Exception ex)
            {
                throw new SessionResumeFailedException("Failed to resume your last session.", ex);
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);

            if (e.Index >= TabPages.Count)
                return;

            var tabPage = TabPages[e.Index];
            var tabRect = GetTabRect(e.Index);
            tabRect.Inflate(-2, -2);

            var closeButtonRectangle = GetTabCloseButtonRectangle(e.Index);
            if (_closeButtonHoveredTabIndex == e.Index)
            {
                if (_isMouseLeftButtonDown)
                {
                    ControlPaint.DrawButton(e.Graphics, closeButtonRectangle, ButtonState.Pushed);
                }
                else
                {
                    ControlPaint.DrawButton(e.Graphics, closeButtonRectangle, ButtonState.Normal);
                }
            }

            closeButtonRectangle.Inflate(-2, -2); //Add some padding inside the button
            e.Graphics.DrawImage(Properties.Resources.close,
                closeButtonRectangle.X,
                closeButtonRectangle.Y,
                closeButtonRectangle.Width,
                closeButtonRectangle.Height);

            var isSelected = SelectedIndex == e.Index;
            var font = isSelected ? new Font(tabPage.Font, FontStyle.Bold) : tabPage.Font;

            TextRenderer.DrawText(e.Graphics, tabPage.Text, font,
                tabRect, tabPage.ForeColor, TextFormatFlags.Left);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            #region Close button drawing
            var tabIndex = CursorHoverTabIndex(e.Location);
            int? closeButtonHoveredTabIndex = null;
            if (tabIndex is not null)
            {
                var closeButtonRectangle = GetTabCloseButtonRectangle(tabIndex.Value);
                if (closeButtonRectangle.Contains(e.Location))
                {
                    closeButtonHoveredTabIndex = tabIndex;
                    _closeButtonHoveredTabRectangle = closeButtonRectangle;
                }
            }

            if (closeButtonHoveredTabIndex != _closeButtonHoveredTabIndex)
            {
                _closeButtonHoveredTabIndex = closeButtonHoveredTabIndex;

                //Trigger redraw to show hover effect, focusing on just the button area to reduce flickering
                if (_closeButtonHoveredTabRectangle is not null)
                    Invalidate(_closeButtonHoveredTabRectangle.Value);
                else
                    Invalidate();
            }
            #endregion

            #region Drag/Drop reordering effect
            if (e.Button == MouseButtons.Left && _dragStartTabIndex is not null && HasMovedPastDragThreshold(e.Location))
            {
                DoDragDrop(TabPages[_dragStartTabIndex.Value], DragDropEffects.Move);
            }
            #endregion
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            _isMouseLeftButtonDown = e.Button == MouseButtons.Left;

            var tabIndex = CursorHoverCloseTabButtonIndex(e.Location);
            if (tabIndex.HasValue)
            {
                //Trigger redraw to show pressed effect
                if (_closeButtonHoveredTabRectangle is not null)
                    Invalidate(_closeButtonHoveredTabRectangle.Value);
                else
                    Invalidate();
            }

            _dragStartTabIndex = CursorHoverTabIndex(e.Location);
            _dragStartLocation = e.Location;
        }

        //Only start dragging once the mouse moves far enough, so normal clicks with a slight wiggle don't start a drag.
        private bool HasMovedPastDragThreshold(Point cursorLocation)
        {
            //Use the default Windows drag threshold (4px)
            var dragSize = SystemInformation.DragSize;

            //Center the rectangle on the drag start location
            var dragThreshold = new Rectangle(
                _dragStartLocation.X - dragSize.Width / 2,
                _dragStartLocation.Y - dragSize.Height / 2,
                dragSize.Width, dragSize.Height);

            //Confirm the cursor has left that rectangle
            return !dragThreshold.Contains(cursorLocation);
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);

            _dragStopTabIndex = null;

            if (_dragStartTabIndex is null)
                return;

            var hoveredTabIndex = CursorHoverTabIndex(PointToClient(new Point(e.X, e.Y)));
            if (hoveredTabIndex is null || hoveredTabIndex == _dragStartTabIndex)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            e.Effect = DragDropEffects.Move;
            _dragStopTabIndex = hoveredTabIndex;
        }

        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);

            if (_dragStartTabIndex is null || _dragStopTabIndex is null)
                return;

            MoveTab(_dragStartTabIndex.Value, _dragStopTabIndex.Value);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Left)
            {
                _isMouseLeftButtonDown = false;

                var tabIndex = CursorHoverCloseTabButtonIndex(e.Location);
                if (tabIndex.HasValue)
                {
                    CloseTab(tabIndex.Value);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                var tabIndex = CursorHoverTabIndex(e.Location);
                if (tabIndex.HasValue)
                {
                    _rightClickedTabIndex = tabIndex.Value;

                    _firstToolStripSeparator.Visible = false;
                    _openQueryLocationToolStripMenuItem.Visible = false;
                    if (TabPages[_rightClickedTabIndex] is CustomTabPage ctp && ctp.IsFileBackedTab)
                    {
                        _firstToolStripSeparator.Visible = true;
                        _openQueryLocationToolStripMenuItem.Visible = true;
                    }

                    _rightClickTabContextMenuStrip.Show(this, e.Location);
                }
            }
            else if (e.Button == MouseButtons.Middle)
            {
                var tabIndex = CursorHoverTabIndex(e.Location);
                if (tabIndex.HasValue)
                {
                    CloseTab(tabIndex.Value);
                }
            }

            _dragStartTabIndex = null;
        }

        private int? CursorHoverTabIndex(Point cursorLocation)
        {
            for (var i = 0; i < TabCount; i++)
            {
                var tabRect = GetTabRect(i);
                if (tabRect.Contains(cursorLocation))
                {
                    return i;
                }
            }

            return null;
        }

        private int? CursorHoverCloseTabButtonIndex(Point cursorLocation)
        {
            for (var i = 0; i < TabCount; i++)
            {
                var closeButtonRectangle = GetTabCloseButtonRectangle(i);
                if (closeButtonRectangle.Contains(cursorLocation))
                {
                    return i;
                }
            }

            return null;
        }

        private Rectangle GetTabCloseButtonRectangle(int tabIndex)
        {
            const int iconSize = 12;
            var tabRect = GetTabRect(tabIndex);
            tabRect.Inflate(-2, -2);

            return new Rectangle(
                tabRect.Right - iconSize - 2,
                tabRect.Top + (tabRect.Height - iconSize) / 2,
                iconSize, iconSize
            );
        }

        /// <summary>
        /// Switches to an existing empty query tab, or creates a new one if there isn't one.
        /// </summary>
        public void NewQuery()
        {
            for (var i = 0; i < TabCount; i++)
            {
                if (IsEmptyQueryEditor(i))
                {
                    SelectedIndex = i;
                    return;
                }
            }

            SelectedIndex = CreateTab();
        }

        /// <summary>
        /// Opens each path in its own tab and selects the last one. Already open .sql files are switched to instead.
        /// </summary>
        /// <param name="paths">.sql files to open, or data files/folders to generate a SELECT query for</param>
        public void OpenFilesOrFolders(params string[] paths)
        {
            if (paths.Length == 0)
                return;

            //An empty query tab at the end gets replaced by the opened files, so the first new query tab can reuse its name
            var emptyLastTab = TabCount > 0 && IsEmptyQueryEditor(TabCount - 1) ? (CustomTabPage)TabPages[TabCount - 1] : null;
            var tabCountBefore = TabCount;

            var tabIndex = -1;
            for (var i = 0; i < paths.Length; i++)
            {
                var scratchNumber = i == 0 ? emptyLastTab?.ScratchNumber : null;
                tabIndex = CreateTab(paths[i], scratchNumber);
            }
            SelectedIndex = tabIndex;

            //Only replace the empty tab if we actually opened something new and didn't switch to an open file
            if (emptyLastTab is not null && TabCount > tabCountBefore)
                TabPages.Remove(emptyLastTab);
        }

        /// <param name="filePath">A .sql file to open, or a data file/folder to generate a SELECT query for</param>
        /// <param name="scratchNumber">Number to use in the "New query N" title. Defaults to the next number.</param>
        /// <returns>The index of the new tab, or of the existing tab if the .sql file is already open</returns>
        private int CreateTab(string? filePath = null, int? scratchNumber = null)
        {
            var hasNoTabs = TabCount == 0;

            CustomTabPage newTab;
            if (File.Exists(filePath) && UtilityMethods.IsSqlFile(filePath))
            {
                //Don't open the same file twice, return the existing tab instead
                var existingTab = TabPages.OfType<CustomTabPage>()
                    .FirstOrDefault(tab => string.Equals(tab.FilePath, filePath, StringComparison.OrdinalIgnoreCase));
                if (existingTab is not null)
                    return TabPages.IndexOf(existingTab);

                newTab = CustomTabPage.ForSqlFile(filePath);
            }
            else
            {
                var number = scratchNumber ?? ++_newQueryCount;
                newTab = filePath is null ? CustomTabPage.ForScratch(number) : CustomTabPage.ForDataSource(number, filePath);
            }

            AddTab(newTab);

            if (hasNoTabs)
            {
                OnSelectedIndexChanged(EventArgs.Empty); //Manually trigger to initialize the tab
            }

            return TabCount - 1;
        }

        public CustomTabPage GetSelectedTab() => (CustomTabPage)(SelectedTab ?? throw new InvalidOperationException("No selected tab found"));

        /// <summary>
        /// Checks if the query editor is "empty". Meaning it's a scratch tab with no query text.
        /// </summary>
        private bool IsEmptyQueryEditor(int tabIndex)
        {
            if (tabIndex >= TabCount)
                throw new ArgumentOutOfRangeException(nameof(tabIndex));

            var tab = (CustomTabPage)TabPages[tabIndex];
            return !tab.IsFileBackedTab && string.IsNullOrWhiteSpace(tab.CurrentText);
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);

            if (SelectedTab is not CustomTabPage selectedTab)
                return;

            //New editors refresh themselves on Load, so only existing ones need it (e.g. to pick up setting changes)
            var isExistingEditor = selectedTab.HasQueryEditor();
            var queryEditor = selectedTab.GetOrCreateQueryEditor();
            if (isExistingEditor)
                queryEditor.RefreshControl();

            queryEditor.Focus();
        }

        //Returns false if the user cancels closing dirty tabs, true otherwise
        public bool CloseTabs(int[] tabIndexes)
        {
            //Check if there are any dirty tabs being closed
            var dirtyTabCount = tabIndexes.Count(i => ((CustomTabPage)TabPages[i]).IsDirty);
            if (dirtyTabCount > 0 && MessageBox.Show(Resources.Strings.CloseDirtyTabsWarningMessageFormat.Format(dirtyTabCount),
                    Resources.Strings.UnsavedChangesWarningTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
            {
                return false;
            }

            //Close tabs in descending order
            foreach (var tabIndex in tabIndexes.OrderByDescending(i => i))
            {
                if (tabIndex < 0 || tabIndex >= TabCount)
                    throw new ArgumentOutOfRangeException(nameof(tabIndexes));

                //Check if there's anything left to close
                if (TabCount == 1 && IsEmptyQueryEditor(0))
                {
                    return tabIndexes.Length > 1;
                }

                TabPages.RemoveAt(tabIndex);
            }

            if (TabCount == 0)
            {
                CreateTab();
            }

            //Last tab should be focused
            SelectedIndex = TabCount - 1;

            Invalidate();
            return true;
        }

        public void CloseTab(int tabIndex)
        {
            if (tabIndex < 0 || tabIndex >= TabCount)
                throw new ArgumentOutOfRangeException(nameof(tabIndex));

            //Don't close if this is already the only, empty, tab
            if (TabCount == 1 && IsEmptyQueryEditor(tabIndex))
            {
                Invalidate(); //Animate the close button
                return;
            }

            var isDirty = ((CustomTabPage)TabPages[tabIndex]).IsDirty;
            if (isDirty && MessageBox.Show(Resources.Strings.CloseDirtyTabWarningMessage, Resources.Strings.UnsavedChangesWarningTitle,
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
            {
                _closeButtonHoveredTabIndex = null; //The dialog took over focus
                Invalidate();
                return;
            }

            TabPages.RemoveAt(tabIndex);

            if (TabCount == 0)
            {
                CreateTab();
            }

            //Last tab should be focused
            SelectedIndex = TabCount - 1;

            Invalidate();
        }

        //All tabs must be added through here so their events get relayed
        private void AddTab(CustomTabPage tab)
        {
            tab.DirtyStateChanged += (s, args) => TabDirtyStateChanged?.Invoke(this, EventArgs.Empty);
            tab.QueryExecuted += (s, args) => QueryExecuted?.Invoke(s, args);
            tab.QueryExecutionStarted += (s, args) => QueryExecutionStarted?.Invoke(s, args);
            tab.DragDrop += (s, args) => OnDragDrop(args);
            TabPages.Add(tab);
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
            e.Control?.Dispose(); //Dispose any removed tabs to free resources
        }

        private void closeTabToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            CloseTab(_rightClickedTabIndex);
        }

        private void closeOtherTabsToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            var tabCount = TabCount;

            if (tabCount <= 1)
                return;

            var tabsToClose = Enumerable.Range(0, TabCount).Except([_rightClickedTabIndex]).ToArray();
            var wasTabsClosed = CloseTabs(tabsToClose);
            if (wasTabsClosed)
                MenuBarClickEvent.FireAndForget(MenuBarClickEvent.ActionId.CloseOtherTabs);
        }

        private void closeAllTabsToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (TabCount == 0)
                return;

            var tabIndexes = Enumerable.Range(0, TabCount).ToArray();
            var wasTabsClosed = CloseTabs(tabIndexes);
            if (wasTabsClosed)
                MenuBarClickEvent.FireAndForget(MenuBarClickEvent.ActionId.CloseAllTabs);
        }

        private void openQueryLocationToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (TabPages[_rightClickedTabIndex] is CustomTabPage ctp
                    && ctp.FilePath is string path
                    && Path.Exists(path)
                    && Path.GetDirectoryName(path) is string dir)
            {
                Process.Start("explorer.exe", $"/select,\"{path}\"");
            }
        }

        private void MoveTab(int tabToBeMovedIndex, int targetTabIndex)
        {
            var tabToBeMoved = TabPages[tabToBeMovedIndex];
            if (targetTabIndex > tabToBeMovedIndex)
            {
                for (var i = tabToBeMovedIndex; i < targetTabIndex; i++)
                {
                    TabPages[i] = TabPages[i + 1];
                }
            }
            else if (targetTabIndex < tabToBeMovedIndex)
            {
                for (var i = tabToBeMovedIndex; i > targetTabIndex; i--)
                {
                    TabPages[i] = TabPages[i - 1];
                }
            }
            else
            {
                return;
            }
            TabPages[targetTabIndex] = tabToBeMoved;
            Controls.SetChildIndex(tabToBeMoved, targetTabIndex); //keep Controls in the same order as TabPages, or RemoveAt(index) removes the wrong tab
            SelectedTab = tabToBeMoved;
            Refresh();
        }

        private void _cacheOpenTabsTimer_Tick(object? sender, EventArgs e)
            => _sessionCache.Save(TabPages.OfType<CustomTabPage>(), SelectedTab as CustomTabPage, _newQueryCount, force: false);

        /// <summary>
        /// Saves the session one last time and stops the periodic saving. Call when the app is closing.
        /// </summary>
        /// <remarks>Saving is synchronous, so we don't need to stop and start the timer on each tick.</remarks>
        public void SaveSessionCacheAndStopSession()
        {
            _cacheOpenTabsTimer.Stop();
            _sessionCache.Save(TabPages.OfType<CustomTabPage>(), SelectedTab as CustomTabPage, _newQueryCount, force: true);
        }
    }
}