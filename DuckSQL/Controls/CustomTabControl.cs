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
        private ContextMenuStrip _rightClickTabContextMenuStrip = new();
        private ToolStripMenuItem _closeTabToolStripMenuItem = new() { Text = Resources.Strings.CloseTabText };
        private ToolStripMenuItem _closeOtherTabsToolStripMenuItem = new() { Text = Resources.Strings.CloseOtherTabsText };
        private ToolStripMenuItem _closeAllTabsToolStripMenuItem = new() { Text = Resources.Strings.CloseAllTabsText };
        private ToolStripSeparator firstToolStripSeparator = new();
        private ToolStripMenuItem _openQueryLocationToolStripMenuItem = new() { Text = Resources.Strings.OpenQueryLocationText };
        #endregion

        #region Drag & Drop
        private int? _dragStartTabIndex = null;
        private int? _dragStopTabIndex = null;
        #endregion

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public EventHandler? QueryExecutionStarted { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public EventHandler<QueryEditorControl.QueryExecutedEventArgs>? QueryExecuted { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public EventHandler? TabDirtyStateChanged { get; set; }

        public CustomTabControl()
        {
            DrawMode = TabDrawMode.OwnerDrawFixed;
            _rightClickTabContextMenuStrip.Items.AddRange(
                [_closeTabToolStripMenuItem, _closeOtherTabsToolStripMenuItem, _closeAllTabsToolStripMenuItem,
                firstToolStripSeparator, _openQueryLocationToolStripMenuItem]);
            _closeTabToolStripMenuItem.Click += closeTabToolStripMenuItem_Click;
            _closeOtherTabsToolStripMenuItem.Click += closeOtherTabsToolStripMenuItem_Click;
            _closeAllTabsToolStripMenuItem.Click += closeAllTabsToolStripMenuItem_Click;
            _openQueryLocationToolStripMenuItem.Click += openQueryLocationToolStripMenuItem_Click;
            AllowDrop = true;
            ShowToolTips = true;
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

            TextRenderer.DrawText(e.Graphics, tabPage.Text, tabPage.Font,
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
            if (e.Button == MouseButtons.Left && _dragStartTabIndex is not null)
            {
                DoDragDrop(TabPages[_dragStartTabIndex.Value], DragDropEffects.Move);
            }
            #endregion

            #region File path tooltip
            if (GetSelectedTab().IsFileBackedTab && tabIndex is not null && closeButtonHoveredTabIndex is null)
            {
                var hoveredTab = TabPages[tabIndex.Value];
                var filePath = hoveredTab.Tag as string;

                if (!string.IsNullOrWhiteSpace(filePath)) //just for extra safety
                {
                    hoveredTab.ToolTipText = filePath;
                }
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

                    firstToolStripSeparator.Visible = false;
                    _openQueryLocationToolStripMenuItem.Visible = false;
                    if (TabPages[_rightClickedTabIndex] is CustomTabPage ctp && ctp.IsFileBackedTab)
                    {
                        firstToolStripSeparator.Visible = true;
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

        public int CreateNewTab(string? queryTextOrFilePath = null, bool skipTabCountIncrement = false)
        {
            var hasNoTabs = TabCount == 0;

            var tabName = Resources.Strings.NewQueryTabTitle;
            var isFileBackedTab = false;

            try
            {
                //Not sure if try..catch is required here, adding it for safety.
                if (File.Exists(queryTextOrFilePath) && Path.GetExtension(queryTextOrFilePath)?.Equals(".sql") == true)
                {
                    tabName = Path.GetFileName(queryTextOrFilePath);
                    isFileBackedTab = true;
                    skipTabCountIncrement = true;
                }
            }
            catch { /*swallow*/ }

            if (!skipTabCountIncrement)
                _newQueryCount++;

            if (_newQueryCount > 1 && !isFileBackedTab)
            {
                tabName += $" {_newQueryCount}";
            }

            var newTab = new CustomTabPage();
            newTab.Text = tabName;
            newTab.Tag = queryTextOrFilePath;
            TabPages.Add(newTab);

            if (hasNoTabs)
            {
                OnSelectedIndexChanged(EventArgs.Empty); //Manually trigger to initialize the tab
            }

            return TabCount - 1;
        }

        public CustomTabPage GetSelectedTab() => (CustomTabPage)(SelectedTab ?? throw new InvalidOperationException("No selected tab found"));

        public int? FindEmptyQueryEditorTabIndex()
        {
            for (var i = 0; i < TabCount; i++)
            {
                if (IsEmptyQueryEditor(i))
                {
                    return i;
                }
            }
            return null;
        }

        /// <summary>
        /// Checks if the query editor is "empty". 
        /// Meaning it has our <see cref="Resources.Strings.NewQueryTabTitle"/> title, no file path, and no query text.
        /// </summary>
        /// <returns></returns>
        public bool IsEmptyQueryEditor(int tabIndex)
        {
            if (tabIndex >= TabCount)
                throw new ArgumentOutOfRangeException(nameof(tabIndex));

            var tab = (CustomTabPage)TabPages[tabIndex];
            if (tab.Controls.Count == 0 || tab.Controls[0] is not QueryEditorControl queryEditor)
            {
                return true;
            }

            var hasQueryText = !string.IsNullOrWhiteSpace(queryEditor.QueryText);
            var hasFilePath = !string.IsNullOrWhiteSpace(queryEditor.CurrentFilePath);

            return tab.HasNewQueryTitle && !hasQueryText && !hasFilePath;
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);

            if (SelectedTab is not CustomTabPage selectedTab)
                return;

            if (selectedTab.Controls.Count == 0)
            {
                //Lazily initialize the query editor
                QueryEditorControl queryEditor;
                var filePathOrQueryText = selectedTab.Tag?.ToString();
                if (!string.IsNullOrWhiteSpace(filePathOrQueryText))
                {
                    if (File.Exists(filePathOrQueryText) || Directory.Exists(filePathOrQueryText))
                    {
                        queryEditor = new QueryEditorControl(filePathOrQueryText) { Dock = DockStyle.Fill };
                    }
                    else
                    {
                        queryEditor = new()
                        {
                            Dock = DockStyle.Fill,
                            QueryText = filePathOrQueryText
                        };
                    }
                }
                else
                {
                    queryEditor = new QueryEditorControl { Dock = DockStyle.Fill };
                }

                AttachEventHandlers(queryEditor, selectedTab);
                selectedTab.Controls.Add(queryEditor);
                queryEditor.Focus();
            }
            else
            {
                var queryEditor = (QueryEditorControl)SelectedTab.Controls[0];
                queryEditor.RefreshControl();
                queryEditor.Focus();
            }
        }

        public void CloseTabs(int[] tabIndexes)
        {
            //Check if there are any dirt tabs being closed
            var dirtyTabCount = tabIndexes.Count(i => TabPages[i].Text.Contains('*'));
            if (dirtyTabCount == 1 && !ConfirmTabClosure(false))
            {
                return;
            }
            else if (dirtyTabCount > 1 && !ConfirmTabClosure(true))
            {
                return;
            }

            //Close tabs in descending order
            foreach (var tabIndex in tabIndexes.OrderByDescending(i => i))
            {
                if (tabIndex < 0 || tabIndex >= TabCount)
                    throw new ArgumentOutOfRangeException(nameof(tabIndex));

                //Check if there's anything left to close
                if (TabCount == 1 && IsEmptyQueryEditor(0))
                {
                    return;
                }

                TabPages.RemoveAt(tabIndex);
            }

            if (TabCount == 0)
            {
                CreateNewTab();
            }

            Invalidate();
        }

        public void CloseAllTabs()
        {
            var tabIndexes = Enumerable.Range(0, TabCount).ToArray();
            CloseTabs(tabIndexes);
        }

        private bool ConfirmTabClosure(bool hasMultipleDirtyTabs)
        {
            if (!hasMultipleDirtyTabs)
            {
                return MessageBox.Show("Are you sure you want to close this tab? Any unsaved changes will be lost.", "Unsaved changes detected",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK;
            }
            else
            {
                return MessageBox.Show("Are you sure you want to close these tabs? Any unsaved changes will be lost.", "Unsaved changes detected",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK;
            }
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

            var isDirty = TabPages[tabIndex].Text.Contains('*');
            if (isDirty && !ConfirmTabClosure(false))
            {
                return;
            }

            TabPages.RemoveAt(tabIndex);

            if (TabCount == 0)
            {
                CreateNewTab();
            }

            Invalidate();
        }

        private void AttachEventHandlers(QueryEditorControl queryEditor, CustomTabPage tab)
        {
            //Handle new tab title styling
            queryEditor.QueryTextChanged += (s, args) =>
            {
                var hasModifiedIndicator = tab.Text.Trim().EndsWith('*');

                if (tab.IsFileBackedTab)
                {
                    //This is a file backed tab. Any changes should make the tab dirty
                    if (!hasModifiedIndicator)
                    {
                        tab.Text = tab.Text.Trim() + " *";
                        Invalidate();
                        TabDirtyStateChanged?.Invoke(this, EventArgs.Empty);
                    }
                    return; 
                }

                var isEmpty = string.IsNullOrWhiteSpace(queryEditor.QueryText);
                if (hasModifiedIndicator && isEmpty)
                {
                    tab.Text = tab.Text.Replace("*", string.Empty).Trim();
                    Invalidate();
                    TabDirtyStateChanged?.Invoke(this, EventArgs.Empty);
                }
                else if (!hasModifiedIndicator && !isEmpty)
                {
                    tab.Text = tab.Text.Trim() + " *";
                    Invalidate();
                    TabDirtyStateChanged?.Invoke(this, EventArgs.Empty);
                }
            };
            queryEditor.QueryExecuted += (s, args) =>
            {
                QueryExecuted?.Invoke(s, args);
            };
            queryEditor.QueryExecutionStarted += (s, args) =>
            {
                QueryExecutionStarted?.Invoke(s, args);
            };
            queryEditor.DragDrop += (s, args) =>
            {
                OnDragDrop(args);
            };
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
            CloseTabs(tabsToClose);
        }

        private void closeAllTabsToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (TabCount == 0)
                return;

            var tabIndexes = Enumerable.Range(0, TabCount).ToArray();
            CloseTabs(tabIndexes);
        }

        private void openQueryLocationToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (TabPages[_rightClickedTabIndex] is CustomTabPage ctp
                    && ctp.Tag is string path
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
            SelectedTab = tabToBeMoved;
            Refresh();
        }
    }
}