using System.ComponentModel;

namespace DuckSQL.Controls
{
    internal class CustomTabControl : TabControl
    {
        private int? _closeButtonHoveredTabIndex = null;
        private bool _isMouseLeftButtonDown = false;
        private int _rightClickedTabIndex = -1;
        private static int _newQueryCount = 0;

        #region Context Menu
        private ContextMenuStrip _closeTabContextMenuStrip = new();
        private ToolStripMenuItem closeTabToolStripMenuItem = new() { Text = Resources.Strings.CloseTabText };
        private ToolStripMenuItem closeOtherTabsToolStripMenuItem = new() { Text = Resources.Strings.CloseOtherTabsText };
        private ToolStripMenuItem closeAllTabsToolStripMenuItem = new() { Text = Resources.Strings.CloseAllTabsText };
        #endregion

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public EventHandler? QueryExecutionStarted { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public EventHandler? QueryExecuted { get; set; }

        public CustomTabControl()
        {
            DrawMode = TabDrawMode.OwnerDrawFixed;
            _closeTabContextMenuStrip.Items.AddRange([closeTabToolStripMenuItem, closeOtherTabsToolStripMenuItem, closeAllTabsToolStripMenuItem]);
            closeTabToolStripMenuItem.Click += closeTabToolStripMenuItem_Click;
            closeOtherTabsToolStripMenuItem.Click += closeOtherTabsToolStripMenuItem_Click;
            closeAllTabsToolStripMenuItem.Click += closeAllTabsToolStripMenuItem_Click;
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

            int? hoveredTabIndex = null;
            for (var i = 0; i < TabCount; i++)
            {
                var closeButtonRectangle = GetTabCloseButtonRectangle(i);
                if (closeButtonRectangle.Contains(e.Location))
                {
                    hoveredTabIndex = i;
                    break;
                }
            }

            if (hoveredTabIndex != _closeButtonHoveredTabIndex)
            {
                _closeButtonHoveredTabIndex = hoveredTabIndex;
                Invalidate(); //Trigger redraw to show hover effect
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            _isMouseLeftButtonDown = e.Button == MouseButtons.Left;

            var tabIndex = CursorHoverCloseTabButtonIndex(e.Location);
            if (tabIndex.HasValue)
            {
                Invalidate(); //Trigger redraw to show pressed effect
            }
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
                    _closeTabContextMenuStrip.Show(this, e.Location);
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

            if (!skipTabCountIncrement)
                _newQueryCount++;

            if (_newQueryCount > 1)
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

            var tab = TabPages[tabIndex];
            if (tab.Controls.Count == 0 || tab.Controls[0] is not QueryEditorControl queryEditor)
            {
                return true;
            }

            var hasNewQueryTitle = tab.Text.StartsWith(Resources.Strings.NewQueryTabTitle);
            var hasQueryText = !string.IsNullOrWhiteSpace(queryEditor.QueryText);
            var hasFilePath = !string.IsNullOrWhiteSpace(queryEditor.CurrentFilePath);

            return hasNewQueryTitle && !hasQueryText && !hasFilePath;
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);

            if (SelectedTab is null)
                return;

            if (SelectedTab.Controls.Count == 0)
            {
                //Lazily initialize the query editor
                QueryEditorControl queryEditor;
                var tag = SelectedTab.Tag?.ToString();
                if (!string.IsNullOrWhiteSpace(tag))
                {
                    if (File.Exists(tag) || Directory.Exists(tag))
                    {
                        queryEditor = new QueryEditorControl(dataFilePath: tag) { Dock = DockStyle.Fill };
                    }
                    else
                    {
                        queryEditor = new()
                        {
                            Dock = DockStyle.Fill,
                            QueryText = tag
                        };
                    }
                }
                else
                {
                    queryEditor = new QueryEditorControl { Dock = DockStyle.Fill };
                }

                AttachEventHandlers(queryEditor, SelectedTab);
                SelectedTab.Controls.Add(queryEditor);
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

        private void AttachEventHandlers(QueryEditorControl queryEditor, TabPage tab)
        {
            //Handle new tab title styling
            queryEditor.QueryTextChanged += (s, args) =>
            {
                if (!tab.Text.StartsWith(Resources.Strings.NewQueryTabTitle))
                {
                    return; //This is a file backed tab. So it will be styled separately
                }

                var isEmpty = string.IsNullOrWhiteSpace(queryEditor.QueryText);
                var hasModifiedIndicator = tab.Text.Trim().EndsWith('*');

                if (hasModifiedIndicator && isEmpty)
                {
                    tab.Text = tab.Text.Replace("*", string.Empty).Trim();
                    Invalidate();
                }
                else if (!hasModifiedIndicator && !isEmpty)
                {
                    tab.Text = tab.Text.Trim() + " *";
                    Invalidate();
                }
            };
            //Tell the main form a query was executed
            queryEditor.QueryExecuted += (s, args) =>
            {
                QueryExecuted?.Invoke(s, args);
            };
            queryEditor.QueryExecutionStarted += (s, args) =>
            {
                QueryExecutionStarted?.Invoke(s, args);
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
    }
}