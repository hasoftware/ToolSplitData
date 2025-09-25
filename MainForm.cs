using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace DataSplitPro
{
    public partial class MainForm : Form
    {
        // Windows API for dark mode title bar
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        // Removed Windows API calls that were causing crashes
        private TextBox txtDelimiter = null!;
        private DataGridView dgvData = null!;
        private Label lblDelimiter = null!;
        private Panel pnlMain = null!;
        // Removed lblDeveloper and pnlBottom - info now in Status Bar
        private ContextMenuStrip contextMenuStrip = null!;
        private OpenFileDialog openFileDialog = null!;
        private SaveFileDialog saveFileDialog = null!;
        private ProgressBar progressBar = null!;
        private Label lblProgress = null!;
        private StatusStrip statusStrip = null!;
        private ToolStripStatusLabel lblTotal = null!;
        private ToolStripStatusLabel lblSelected = null!;
        private ToolStripStatusLabel lblBlackout = null!;
        private ToolStripStatusLabel lblColumns = null!;
        private ToolStripStatusLabel lblSeparator1 = null!;
        private ToolStripStatusLabel lblSeparator2 = null!;
        private ToolStripStatusLabel lblSeparator3 = null!;
        private ToolStripStatusLabel lblLogo = null!;
        private ToolStripStatusLabel lblDev = null!;
        private ToolStripStatusLabel lblTelegram = null!;
        private ToolStripStatusLabel lblChannel = null!;
        private ToolStripStatusLabel lblGithub = null!;
        private CancellationTokenSource? cancellationTokenSource = null;

        // Export functionality controls
        private TextBox txtExportFormat = null!;
        private Button[] columnButtons = new Button[16]; // Column1 to Column16
        private Button btnClearExport = null!;
        private Label lblExportFormat = null!;
        private Panel pnlExportSeparator = null!;
        private Panel pnlDelimiterSeparator = null!;

        public MainForm()
        {
            try
            {
                Console.WriteLine("MainForm constructor started");

                // Set application icon - simplified for stability
                try
                {
                    if (File.Exists("hasoftware.ico"))
                    {
                        // Simple approach - just load the icon file directly
                        this.Icon = new Icon("hasoftware.ico");
                        Console.WriteLine("Icon loaded successfully");
                    }
                    else
                    {
                        Console.WriteLine("Icon file not found: hasoftware.ico");
                    }
                }
                catch (Exception iconEx)
                {
                    Console.WriteLine($"Error loading icon: {iconEx.Message}");
                    // Continue without icon if there's an error
                }

                InitializeComponent();

                // Enable dark mode for title bar
                EnableDarkMode();

                Console.WriteLine("MainForm constructor completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MainForm constructor: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private void EnableDarkMode()
        {
            try
            {
                // Only try dark mode if handle is valid
                if (this.Handle != IntPtr.Zero)
                {
                    // Enable dark mode for title bar
                    int darkMode = 1;
                    int result = DwmSetWindowAttribute(this.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref darkMode, sizeof(int));

                    if (result != 0)
                    {
                        // Fallback for older Windows versions
                        result = DwmSetWindowAttribute(this.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1, ref darkMode, sizeof(int));
                    }

                    Console.WriteLine($"Dark mode result: {result}");
                }
                else
                {
                    Console.WriteLine("Handle not ready, skipping dark mode");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enabling dark mode: {ex.Message}");
                // Continue without dark mode if there's an error
            }
        }

        private void InitializeComponent()
        {
            try
            {
                Console.WriteLine("InitializeComponent started");
                this.SuspendLayout();

                // Form properties
                this.Text = "Data Split Pro v1.0 - HASOFTWARE";
                this.Size = new Size(1000, 700);
                this.StartPosition = FormStartPosition.CenterScreen;
                this.MinimumSize = new Size(800, 600);
                this.BackColor = Color.FromArgb(45, 45, 48);
                this.WindowState = FormWindowState.Maximized; // Start maximized

                // Main panel
                pnlMain = new Panel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(20)
                };



                // Delimiter section (moved down)
                lblDelimiter = new Label
                {
                    Text = "Ký tự ngăn cách:",
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.White,
                    AutoSize = true,
                    Location = new Point(20, 180)
                };

                txtDelimiter = new TextBox
                {
                    Text = "|",
                    Font = new Font("Consolas", 10),
                    Location = new Point(140, 180),
                    Size = new Size(50, 25),
                    BackColor = Color.FromArgb(60, 60, 63),
                    ForeColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };

                // Export format section
                lblExportFormat = new Label
                {
                    Text = "📊 Data Export",
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    ForeColor = Color.FromArgb(100, 200, 255),
                    AutoSize = true,
                    Location = new Point(20, 20)
                };

                txtExportFormat = new TextBox
                {
                    Font = new Font("Consolas", 10),
                    Location = new Point(20, 45),
                    Size = new Size(250, 25),
                    BackColor = Color.FromArgb(60, 60, 63),
                    ForeColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    PlaceholderText = "VD: Column1|Column3|Column5..."
                };

                btnClearExport = new Button
                {
                    Text = "Clear",
                    Font = new Font("Segoe UI", 9),
                    Location = new Point(280, 45),
                    Size = new Size(60, 25),
                    BackColor = Color.FromArgb(220, 53, 69),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnClearExport.FlatAppearance.BorderSize = 0;
                btnClearExport.Click += BtnClearExport_Click;

                // Copy Export button
                var btnCopyExport = new Button
                {
                    Text = "Copy",
                    Font = new Font("Segoe UI", 9),
                    Location = new Point(350, 45),
                    Size = new Size(60, 25),
                    BackColor = Color.FromArgb(0, 122, 204),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnCopyExport.FlatAppearance.BorderSize = 0;
                btnCopyExport.Click += BtnCopyExport_Click;

                // Export to file button
                var btnExportFile = new Button
                {
                    Text = "Export",
                    Font = new Font("Segoe UI", 9),
                    Location = new Point(420, 45),
                    Size = new Size(60, 25),
                    BackColor = Color.FromArgb(40, 167, 69),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnExportFile.FlatAppearance.BorderSize = 0;
                btnExportFile.Click += BtnExportFile_Click;

                // Export section separator
                pnlExportSeparator = new Panel
                {
                    Location = new Point(20, 75),
                    Size = new Size(460, 2),
                    BackColor = Color.FromArgb(100, 200, 255)
                };

                // Column buttons
                for (int i = 0; i < 16; i++)
                {
                    columnButtons[i] = new RoundedButton
                    {
                        Text = $"Column{i + 1}",
                        Font = new Font("Segoe UI", 8),
                        Location = new Point(20 + (i % 8) * 70, 80 + (i / 8) * 30),
                        Size = new Size(65, 25),
                        BackColor = Color.White,
                        ForeColor = Color.FromArgb(0, 122, 204),
                        Cursor = Cursors.Hand,
                        Tag = i + 1 // Store column number
                    };
                    columnButtons[i].Click += ColumnButton_Click;
                }

                // Stats label (removed - moved to status bar)

                // Progress bar
                progressBar = new ProgressBar
                {
                    Location = new Point(20, 220),
                    Size = new Size(600, 20),
                    Style = ProgressBarStyle.Marquee,
                    Visible = false
                };

                // Progress label
                lblProgress = new Label
                {
                    Text = "Đang xử lý dữ liệu...",
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.FromArgb(100, 200, 100),
                    AutoSize = true,
                    Location = new Point(630, 223),
                    Visible = false
                };



                // Delimiter section separator
                pnlDelimiterSeparator = new Panel
                {
                    Location = new Point(20, 210),
                    Size = new Size(200, 2),
                    BackColor = Color.FromArgb(100, 100, 100)
                };

                // DataGridView
                dgvData = new DataGridView
                {
                    Location = new Point(20, 230),
                    Size = new Size(940, 400),
                    BackgroundColor = Color.FromArgb(45, 45, 48), // Darker background like the image
                    ForeColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    ReadOnly = true,
                    SelectionMode = DataGridViewSelectionMode.CellSelect,
                    MultiSelect = true,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                    ScrollBars = ScrollBars.Both, // Enable both horizontal and vertical scrollbars
                    ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                    {
                        BackColor = Color.FromArgb(63, 63, 70), // Slightly lighter header like the image
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 10, FontStyle.Bold),
                        Alignment = DataGridViewContentAlignment.MiddleLeft,
                        SelectionBackColor = Color.FromArgb(63, 63, 70),
                        SelectionForeColor = Color.White
                    },
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        BackColor = Color.FromArgb(45, 45, 48), // Darker cell background
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 9), // Changed to Segoe UI for better readability
                        SelectionBackColor = Color.FromArgb(0, 120, 215), // Blue selection like the image
                        SelectionForeColor = Color.White,
                        Alignment = DataGridViewContentAlignment.MiddleLeft
                    },
                    AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                    {
                        BackColor = Color.FromArgb(50, 50, 53), // Slightly different for alternating rows
                        ForeColor = Color.White,
                        SelectionBackColor = Color.FromArgb(0, 120, 215),
                        SelectionForeColor = Color.White
                    },
                    RowHeadersVisible = false,
                    VirtualMode = false,
                    EnableHeadersVisualStyles = false,
                    AllowUserToResizeRows = false,
                    RowTemplate = { Height = 28 }, // Slightly taller rows
                    CellBorderStyle = DataGridViewCellBorderStyle.Single, // Both horizontal and vertical lines
                    ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                    RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                    GridColor = Color.FromArgb(38, 38, 38) // Very subtle vertical lines (15% opacity)
                };

                // Scrollbar customization removed to fix crashes

                // Status Strip (VS Code style)
                Console.WriteLine("Creating StatusStrip...");
                statusStrip = new StatusStrip
                {
                    BackColor = Color.FromArgb(37, 37, 38), // VS Code status bar color
                    ForeColor = Color.FromArgb(204, 204, 204), // VS Code text color
                    Font = new Font("Segoe UI", 9),
                    Height = 22,
                    SizingGrip = false
                };
                Console.WriteLine("StatusStrip created successfully");

                // Total items label
                lblTotal = new ToolStripStatusLabel
                {
                    Text = "Total: 0",
                    ForeColor = Color.FromArgb(204, 204, 204),
                    Font = new Font("Segoe UI", 9),
                    AutoSize = true
                };

                // Separator 1
                lblSeparator1 = new ToolStripStatusLabel
                {
                    Text = "|",
                    ForeColor = Color.FromArgb(128, 128, 128),
                    Font = new Font("Segoe UI", 9)
                };

                // Selected items label
                lblSelected = new ToolStripStatusLabel
                {
                    Text = "Selected: 0",
                    ForeColor = Color.FromArgb(204, 204, 204),
                    Font = new Font("Segoe UI", 9),
                    AutoSize = true
                };

                // Separator 2
                lblSeparator2 = new ToolStripStatusLabel
                {
                    Text = "|",
                    ForeColor = Color.FromArgb(128, 128, 128),
                    Font = new Font("Segoe UI", 9)
                };

                // Blackout items label
                lblBlackout = new ToolStripStatusLabel
                {
                    Text = "Black out: 0",
                    ForeColor = Color.FromArgb(204, 204, 204),
                    Font = new Font("Segoe UI", 9),
                    AutoSize = true
                };

                // Separator 3
                lblSeparator3 = new ToolStripStatusLabel
                {
                    Text = "|",
                    ForeColor = Color.FromArgb(128, 128, 128),
                    Font = new Font("Segoe UI", 9)
                };

                // Columns label
                lblColumns = new ToolStripStatusLabel
                {
                    Text = "Columns: 0",
                    ForeColor = Color.FromArgb(204, 204, 204),
                    Font = new Font("Segoe UI", 9),
                    AutoSize = true
                };

                // Logo label (VS Code style)
                // Load and resize logo to fit status bar height
                var logoImage = Image.FromFile("hasoftware.ico");
                var resizedLogo = new Bitmap(logoImage, new Size(32, 14)); // Very small to fit status bar

                lblLogo = new ToolStripStatusLabel
                {
                    Text = "",
                    ForeColor = Color.FromArgb(0, 120, 215),
                    Font = new Font("Segoe UI", 8),
                    AutoSize = true,
                    Margin = new Padding(1, 0, 3, 0),
                    Image = resizedLogo,
                    ImageScaling = ToolStripItemImageScaling.None // Prevent automatic scaling
                };

                // Developer info labels
                lblDev = new ToolStripStatusLabel
                {
                    Text = "Dev: Trịnh Hoàng Anh",
                    ForeColor = Color.FromArgb(204, 204, 204),
                    Font = new Font("Segoe UI", 9),
                    AutoSize = true,
                    IsLink = true,
                    LinkColor = Color.FromArgb(70, 150, 255), // Moderate blue for stability
                    ActiveLinkColor = Color.FromArgb(100, 180, 255), // Brighter on hover
                    Margin = new Padding(10, 0, 5, 0)
                };

                lblTelegram = new ToolStripStatusLabel
                {
                    Text = "Telegram: HoangAnhDev",
                    ForeColor = Color.FromArgb(204, 204, 204),
                    Font = new Font("Segoe UI", 9),
                    AutoSize = true,
                    IsLink = true,
                    LinkColor = Color.FromArgb(70, 150, 255), // Moderate blue for stability
                    ActiveLinkColor = Color.FromArgb(100, 180, 255), // Brighter on hover
                    Margin = new Padding(5, 0, 5, 0)
                };

                lblChannel = new ToolStripStatusLabel
                {
                    Text = "Channel: HASOFTWARE",
                    ForeColor = Color.FromArgb(204, 204, 204),
                    Font = new Font("Segoe UI", 9),
                    AutoSize = true,
                    IsLink = true,
                    LinkColor = Color.FromArgb(70, 150, 255), // Moderate blue for stability
                    ActiveLinkColor = Color.FromArgb(100, 180, 255), // Brighter on hover
                    Margin = new Padding(5, 0, 5, 0)
                };

                lblGithub = new ToolStripStatusLabel
                {
                    Text = "Github: HASOFTWARE",
                    ForeColor = Color.FromArgb(204, 204, 204),
                    Font = new Font("Segoe UI", 9),
                    AutoSize = true,
                    IsLink = true,
                    LinkColor = Color.FromArgb(70, 150, 255), // Moderate blue for stability
                    ActiveLinkColor = Color.FromArgb(100, 180, 255), // Brighter on hover
                    Margin = new Padding(5, 0, 10, 0)
                };

                // Create spacer for right alignment
                var spacer = new ToolStripStatusLabel
                {
                    Spring = true,
                    Text = ""
                };

                // Add labels to status strip step by step to avoid errors
                Console.WriteLine("Adding labels to StatusStrip...");
                try
                {
                    statusStrip.Items.Add(lblLogo);
                    Console.WriteLine("Added lblLogo");
                    statusStrip.Items.Add(lblTotal);
                    Console.WriteLine("Added lblTotal");
                    statusStrip.Items.Add(lblSeparator1);
                    Console.WriteLine("Added lblSeparator1");
                    statusStrip.Items.Add(lblSelected);
                    Console.WriteLine("Added lblSelected");
                    statusStrip.Items.Add(lblSeparator2);
                    Console.WriteLine("Added lblSeparator2");
                    statusStrip.Items.Add(lblBlackout);
                    Console.WriteLine("Added lblBlackout");
                    statusStrip.Items.Add(lblSeparator3);
                    Console.WriteLine("Added lblSeparator3");
                    statusStrip.Items.Add(lblColumns);
                    Console.WriteLine("Added lblColumns");
                    statusStrip.Items.Add(spacer); // Spacer to push developer info to the right
                    Console.WriteLine("Added spacer");
                    statusStrip.Items.Add(lblDev);
                    Console.WriteLine("Added lblDev");
                    statusStrip.Items.Add(lblTelegram);
                    Console.WriteLine("Added lblTelegram");
                    statusStrip.Items.Add(lblChannel);
                    Console.WriteLine("Added lblChannel");
                    statusStrip.Items.Add(lblGithub);
                    Console.WriteLine("Added lblGithub");
                    Console.WriteLine("All labels added successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding labels to StatusStrip: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    throw;
                }

                // Add click events for links
                lblDev.Click += LblDev_Click;
                lblTelegram.Click += LblTelegram_Click;
                lblChannel.Click += LblChannel_Click;
                lblGithub.Click += LblGithub_Click;

                // Bottom panel removed - developer info now in Status Bar

                // Add controls to panels
                var allControls = new List<Control>
                {
                    lblDelimiter, txtDelimiter, pnlDelimiterSeparator,
                    lblExportFormat, pnlExportSeparator, txtExportFormat, btnClearExport, btnCopyExport, btnExportFile,
                    progressBar, lblProgress, dgvData
                };

                // Add column buttons
                allControls.AddRange(columnButtons);

                pnlMain.Controls.AddRange(allControls.ToArray());
                this.Controls.Add(pnlMain);
                this.Controls.Add(statusStrip);


                // Add keyboard shortcuts
                this.KeyPreview = true;
                this.KeyDown += MainForm_KeyDown;

                // Removed scrollbar theme loading

                // Initialize file dialogs
                InitializeFileDialogs();

                // Initialize context menu
                InitializeContextMenu();

                // Add event handlers once
                dgvData.ColumnHeaderMouseClick += DgvData_ColumnHeaderMouseClick;
                dgvData.CellClick += DgvData_CellClick;
                dgvData.SelectionChanged += DgvData_SelectionChanged;
                this.Resize += MainForm_Resize;

                this.ResumeLayout(false);
                Console.WriteLine("InitializeComponent completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in InitializeComponent: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }


        private void MainForm_KeyDown(object? sender, KeyEventArgs e)
        {
            try
            {
                // Ctrl+A: Select all cells (highlight)
                if (e.Control && e.KeyCode == Keys.A)
                {
                    if (dgvData.Rows.Count > 0)
                    {
                        dgvData.SelectAll();
                        e.Handled = true;
                    }
                }
                // Ctrl+D: Clear selection (unhighlight)
                else if (e.Control && e.KeyCode == Keys.D)
                {
                    dgvData.ClearSelection();
                    e.Handled = true;
                }
                // Delete: Delete selected rows
                else if (e.KeyCode == Keys.Delete)
                {
                    if (dgvData.SelectedCells.Count > 0)
                    {
                        DeleteSelectedRows();
                        e.Handled = true;
                    }
                }
                // Ctrl+C: Copy selected data
                else if (e.Control && e.KeyCode == Keys.C)
                {
                    if (dgvData.SelectedCells.Count > 0)
                    {
                        CopySelectedData();
                        e.Handled = true;
                    }
                }
                // Space: Toggle checkboxes for selected rows
                else if (e.KeyCode == Keys.Space)
                {
                    if (dgvData.SelectedCells.Count > 0 && dgvData.Columns.Count > 1)
                    {
                        ToggleCheckboxesForSelectedRows();
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xử lý phím tắt: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void InitializeFileDialogs()
        {
            openFileDialog = new OpenFileDialog
            {
                Title = "Chọn file để import",
                Filter = "Text Files (*.txt)|*.txt|CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                DefaultExt = "txt"
            };

            saveFileDialog = new SaveFileDialog
            {
                Title = "Lưu file dữ liệu",
                Filter = "Text Files (*.txt)|*.txt|CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                DefaultExt = "txt"
            };
        }

        private void InitializeContextMenu()
        {
            contextMenuStrip = new ContextMenuStrip
            {
                BackColor = Color.FromArgb(45, 45, 48), // VS Code exact background
                ForeColor = Color.FromArgb(212, 212, 212), // VS Code exact text color
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                RenderMode = ToolStripRenderMode.System,
                ShowImageMargin = false,
                ShowCheckMargin = false,
                DropShadowEnabled = true,
                CanOverflow = false,
                Padding = new Padding(0, 0, 0, 0)
            };

            // Add rounded corners effect
            contextMenuStrip.Paint += (s, e) =>
            {
                var rect = new Rectangle(0, 0, contextMenuStrip.Width - 1, contextMenuStrip.Height - 1);
                using (var path = GetRoundedRectanglePath(rect, 8))
                {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    e.Graphics.FillPath(new SolidBrush(Color.FromArgb(45, 45, 48)), path);
                    e.Graphics.DrawPath(new Pen(Color.FromArgb(60, 60, 60), 1), path);
                }
            };

            // Paste Data submenu
            var pasteDataItem = CreateWindows11MenuItem("Paste Data", null);
            var pasteClearOldItem = CreateWindows11MenuItem("Xóa data cũ", null, PasteFromClipboard_Click);
            var pasteKeepOldItem = CreateWindows11MenuItem("Không xóa data cũ", null, PasteFromClipboardKeepOld_Click);

            pasteDataItem.DropDownItems.AddRange(new ToolStripItem[] { pasteClearOldItem, pasteKeepOldItem });

            // Import Data từ File submenu
            var importDataItem = CreateWindows11MenuItem("Import Data từ File", null);
            var importClearOldItem = CreateWindows11MenuItem("Xóa data cũ", null, ImportFile_Click);
            var importKeepOldItem = CreateWindows11MenuItem("Không xóa data cũ", null, ImportFileKeepOld_Click);

            importDataItem.DropDownItems.AddRange(new ToolStripItem[] { importClearOldItem, importKeepOldItem });


            // Separator
            var separator1 = new ToolStripSeparator
            {
                BackColor = Color.FromArgb(85, 85, 85), // VS Code exact separator color
                Margin = new Padding(0, 1, 0, 1)
            };

            // Tích chọn submenu
            var selectItem = CreateWindows11MenuItem("Tích chọn", null);
            var selectAllItem = CreateWindows11MenuItem("Chọn tất cả", null, SelectAll_Click);
            var selectHighlightedItem = CreateWindows11MenuItem("Chỉ chọn dòng bôi đen", null, SelectHighlighted_Click);

            selectItem.DropDownItems.AddRange(new ToolStripItem[] { selectAllItem, selectHighlightedItem });

            // Bỏ chọn submenu
            var deselectItem = CreateWindows11MenuItem("Bỏ chọn", null);
            var deselectAllItem = CreateWindows11MenuItem("Bỏ chọn tất cả", null, DeselectAll_Click);
            var deselectHighlightedItem = CreateWindows11MenuItem("Chỉ bỏ chọn dòng bôi đen", null, DeselectHighlighted_Click);

            deselectItem.DropDownItems.AddRange(new ToolStripItem[] { deselectAllItem, deselectHighlightedItem });

            // Separator
            var separator2 = new ToolStripSeparator
            {
                BackColor = Color.FromArgb(85, 85, 85), // VS Code exact separator color
                Margin = new Padding(0, 1, 0, 1)
            };

            // Xóa Data submenu
            var deleteDataItem = CreateWindows11MenuItem("Xóa Data", null);
            var deleteAllItem = CreateWindows11MenuItem("Xóa toàn bộ", null, DeleteAllData_Click);
            var deleteHighlightedItem = CreateWindows11MenuItem("Chỉ xóa dòng bôi đen", null, DeleteHighlighted_Click);
            var deleteSelectedItem = CreateWindows11MenuItem("Chỉ xóa dòng tích chọn", null, DeleteSelected_Click);
            var deleteNotHighlightedItem = CreateWindows11MenuItem("Chỉ xóa dòng không bôi đen", null, DeleteNotHighlighted_Click);
            var deleteNotSelectedItem = CreateWindows11MenuItem("Chỉ xóa dòng không tích chọn", null, DeleteNotSelected_Click);

            deleteDataItem.DropDownItems.AddRange(new ToolStripItem[] {
                deleteAllItem,
                deleteHighlightedItem,
                deleteSelectedItem,
                deleteNotHighlightedItem,
                deleteNotSelectedItem
            });

            // Add items to context menu
            contextMenuStrip.Items.AddRange(new ToolStripItem[]
            {
                pasteDataItem,
                importDataItem,
                separator1,
                selectItem,
                deselectItem,
                separator2,
                deleteDataItem
            });

            // Assign context menu to DataGridView
            dgvData.ContextMenuStrip = contextMenuStrip;
        }

        private async void PasteFromClipboard_Click(object? sender, EventArgs e)
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    string clipboardText = Clipboard.GetText();

                    // Show progress
                    ShowProgress(true, "Đang xử lý dữ liệu từ clipboard...");

                    // Process data asynchronously
                    await ProcessDataAsync(clipboardText, txtDelimiter.Text);

                    int lineCount = clipboardText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;

                    // Show message box asynchronously to avoid blocking UI
                    await Task.Run(() =>
                    {
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() =>
                            {
                                MessageBox.Show($"Đã paste dữ liệu từ clipboard!\nSố dòng: {lineCount}", "Thành công",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }));
                        }
                    });
                }
                else
                {
                    MessageBox.Show("Clipboard không chứa dữ liệu text!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi paste từ clipboard: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ShowProgress(false);
            }
        }

        private async void ImportFile_Click(object? sender, EventArgs e)
        {
            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Show progress
                    ShowProgress(true, "Đang đọc file...");

                    // Read file asynchronously
                    string fileContent = await Task.Run(() => File.ReadAllText(openFileDialog.FileName, Encoding.UTF8));

                    // Process data asynchronously
                    ShowProgress(true, "Đang xử lý dữ liệu...");
                    await ProcessDataAsync(fileContent, txtDelimiter.Text);

                    int lineCount = fileContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;

                    // Show message box asynchronously to avoid blocking UI
                    await Task.Run(() =>
                    {
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() =>
                            {
                                MessageBox.Show($"Đã import file: {Path.GetFileName(openFileDialog.FileName)}\nSố dòng: {lineCount}", "Thành công",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }));
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi import file: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ShowProgress(false);
            }
        }


        private void ClearSelectedRows_Click(object? sender, EventArgs e)
        {
            try
            {
                // Count checked rows
                int checkedCount = 0;
                foreach (DataGridViewRow row in dgvData.Rows)
                {
                    if (row.Cells.Count > 1 && row.Cells[1].Value is bool isChecked && isChecked)
                    {
                        checkedCount++;
                    }
                }

                if (checkedCount > 0)
                {
                    DialogResult result = MessageBox.Show($"Bạn có chắc muốn xóa {checkedCount} dòng đã tích chọn?",
                        "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // Remove checked rows (from bottom to top to avoid index issues)
                        for (int i = dgvData.Rows.Count - 1; i >= 0; i--)
                        {
                            DataGridViewRow row = dgvData.Rows[i];
                            if (row.Cells.Count > 1 && row.Cells[1].Value is bool isChecked && isChecked)
                            {
                                dgvData.Rows.RemoveAt(i);
                            }
                        }
                        MessageBox.Show($"Đã xóa {checkedCount} dòng đã tích chọn!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Update status bar after deletion
                        UpdateStatusBar();
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng tích chọn dòng cần xóa!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa dòng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearAll_Click(object? sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa tất cả dữ liệu?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                dgvData.DataSource = null;
                dgvData.Columns.Clear();
                UpdateStats(0, 0);
                UpdateStatusBar();
            }
        }

        private void CopySelectedData_Click(object? sender, EventArgs e)
        {
            CopySelectedData();
        }


        private void BtnCopyExport_Click(object? sender, EventArgs e)
        {
            ExportSelectedData();
        }

        private void BtnExportFile_Click(object? sender, EventArgs e)
        {
            ExportSelectedDataToFile();
        }

        private void SelectAll_Click(object? sender, EventArgs e)
        {
            try
            {
                if (dgvData.Rows.Count > 0 && dgvData.Columns.Count > 1)
                {
                    // Check all checkboxes (column index 1)
                    foreach (DataGridViewRow row in dgvData.Rows)
                    {
                        if (row.Cells.Count > 1)
                        {
                            row.Cells[1].Value = true;
                        }
                    }
                    UpdateStatusBar();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SelectAll: {ex.Message}");
            }
        }

        private void DeselectAll_Click(object? sender, EventArgs e)
        {
            try
            {
                if (dgvData.Rows.Count > 0 && dgvData.Columns.Count > 1)
                {
                    // Uncheck all checkboxes (column index 1)
                    foreach (DataGridViewRow row in dgvData.Rows)
                    {
                        if (row.Cells.Count > 1)
                        {
                            row.Cells[1].Value = false;
                        }
                    }
                    UpdateStatusBar();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in DeselectAll: {ex.Message}");
            }
        }

        private void ToggleCheckboxesForSelectedRows()
        {
            try
            {
                if (dgvData.SelectedCells.Count == 0 || dgvData.Columns.Count <= 1)
                    return;

                // Get unique row indices from selected cells
                var selectedRowIndices = dgvData.SelectedCells
                    .Cast<DataGridViewCell>()
                    .Select(cell => cell.RowIndex)
                    .Distinct()
                    .Where(rowIndex => rowIndex >= 0 && rowIndex < dgvData.Rows.Count)
                    .ToList();

                if (selectedRowIndices.Count == 0)
                    return;

                // Count how many selected rows are currently checked
                int checkedCount = 0;
                foreach (int rowIndex in selectedRowIndices)
                {
                    if (dgvData.Rows[rowIndex].Cells.Count > 1)
                    {
                        if (dgvData.Rows[rowIndex].Cells[1].Value is bool isChecked && isChecked)
                        {
                            checkedCount++;
                        }
                    }
                }

                // If more than half are checked, uncheck all; otherwise check all
                bool shouldCheckAll = checkedCount < selectedRowIndices.Count / 2;

                // Apply to all selected rows
                foreach (int rowIndex in selectedRowIndices)
                {
                    if (dgvData.Rows[rowIndex].Cells.Count > 1)
                    {
                        dgvData.Rows[rowIndex].Cells[1].Value = shouldCheckAll;
                    }
                }

                UpdateStatusBar();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ToggleCheckboxesForSelectedRows: {ex.Message}");
            }
        }

        private void DgvData_SelectionChanged(object? sender, EventArgs e)
        {
            UpdateStatusBar();
        }

        // Removed scrollbar event handler that was causing crashes

        // Removed MainForm_Load scrollbar customization

        private void LblDev_Click(object? sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://www.facebook.com/100007353125377",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening Facebook link: {ex.Message}");
            }
        }

        private void LblTelegram_Click(object? sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://t.me/Hoanganhdev",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening Telegram link: {ex.Message}");
            }
        }

        private void LblChannel_Click(object? sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://t.me/hasoftware",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening Channel link: {ex.Message}");
            }
        }

        private void LblGithub_Click(object? sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://github.com/hasoftware",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening GitHub link: {ex.Message}");
            }
        }

        private void UpdateStatusBar()
        {
            try
            {
                if (statusStrip == null) return;

                // Update Total
                int totalRows = dgvData.Rows.Count;
                lblTotal.Text = $"Total: {totalRows}";

                // Update Selected (checked checkboxes)
                int selectedCount = 0;
                if (dgvData.Columns.Count > 1) // Check if checkbox column exists
                {
                    foreach (DataGridViewRow row in dgvData.Rows)
                    {
                        if (row.Cells.Count > 1 && row.Cells[1].Value is bool isChecked && isChecked)
                        {
                            selectedCount++;
                        }
                    }
                }
                lblSelected.Text = $"Selected: {selectedCount}";

                // Update Blackout (highlighted/selected rows - count unique rows only)
                int blackoutCount = 0;
                if (dgvData.SelectedCells.Count > 0)
                {
                    // Get unique row indices from selected cells
                    var selectedRowIndices = dgvData.SelectedCells
                        .Cast<DataGridViewCell>()
                        .Select(cell => cell.RowIndex)
                        .Distinct()
                        .Where(rowIndex => rowIndex >= 0 && rowIndex < dgvData.Rows.Count)
                        .Count();
                    blackoutCount = selectedRowIndices;
                }
                lblBlackout.Text = $"Black out: {blackoutCount}";

                // Update Columns (data columns only, excluding STT and Checkbox)
                int dataColumns = Math.Max(0, dgvData.Columns.Count - 2);
                lblColumns.Text = $"Columns: {dataColumns}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating status bar: {ex.Message}");
            }
        }

        private async Task ProcessDataAsync(string inputText, string delimiter)
        {
            try
            {
                // Show progress
                ShowProgress(true, "Đang tách dữ liệu...");

                // Cancel previous operation if any
                cancellationTokenSource?.Cancel();
                cancellationTokenSource = new CancellationTokenSource();

                // Process data in background thread with progress reporting
                var result = await Task.Run(() => ProcessDataWithProgress(inputText, delimiter, cancellationTokenSource.Token));

                if (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    // Update UI on main thread
                    await UpdateUIAsync(result);
                }
            }
            catch (OperationCanceledException)
            {
                // Operation was cancelled, do nothing
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xử lý dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ShowProgress(false);
            }
        }

        private void MainForm_Resize(object? sender, EventArgs e)
        {
            try
            {
                if (this.WindowState == FormWindowState.Maximized || this.WindowState == FormWindowState.Normal)
                {
                    ResizeControls();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in resize: {ex.Message}");
            }
        }

        private void ResizeControls()
        {
            try
            {
                // Get current form size
                int formWidth = this.ClientSize.Width;
                int formHeight = this.ClientSize.Height;

                // Calculate available space (minus padding and status bar)
                int availableWidth = formWidth - 40; // 20px padding on each side
                int statusBarHeight = statusStrip?.Height ?? 22;
                int availableHeight = formHeight - statusBarHeight - 80; // Status bar + padding + 0.5cm gap (20px)

                // Export format textbox stays fixed width (250px)
                // Buttons stay in fixed positions relative to textbox

                // Resize DataGridView to fill remaining space with small gap from status bar
                if (dgvData != null)
                {
                    dgvData.Width = availableWidth;
                    dgvData.Height = Math.Max(200, availableHeight - 120); // Min height 200, leave space for controls above
                }

                // Resize progress bar
                if (progressBar != null)
                {
                    progressBar.Width = Math.Max(600, availableWidth - 200);
                }

                // Reposition progress label
                if (lblProgress != null && progressBar != null)
                {
                    lblProgress.Location = new Point(progressBar.Right + 10, lblProgress.Top);
                }

                // Stats label removed - now in status bar

                // Auto-resize DataGridView columns if data exists
                if (dgvData != null && dgvData.Rows.Count > 0)
                {
                    AutoResizeDataGridViewColumns();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ResizeControls: {ex.Message}");
            }
        }

        private void AutoResizeDataGridViewColumns()
        {
            try
            {
                if (dgvData.Columns.Count == 0) return;

                // Calculate available width for data columns (excluding STT and Checkbox columns)
                int availableWidth = dgvData.Width - 140; // 80px for STT + 60px for Checkbox
                int dataColumnsCount = dgvData.Columns.Count - 2; // Exclude STT and Checkbox columns

                if (dataColumnsCount > 0)
                {
                    int columnWidth = Math.Max(120, availableWidth / dataColumnsCount); // Min width 120px

                    // Set width for data columns (starting from index 2)
                    for (int i = 2; i < dgvData.Columns.Count; i++)
                    {
                        dgvData.Columns[i].Width = columnWidth;
                        dgvData.Columns[i].DefaultCellStyle.Font = new Font("Segoe UI", 9);
                        dgvData.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AutoResizeDataGridViewColumns: {ex.Message}");
            }
        }

        private void UpdateStats(int rowCount, int columnCount)
        {
            // Stats are now handled in UpdateStatusBar method
            UpdateStatusBar();
        }

        // Removed scrollbar customization methods that were causing crashes

        private void EnsureSTTColumn()
        {
            if (dgvData.Columns.Count > 0)
            {
                // Force STT column properties
                dgvData.Columns[0].HeaderText = "STT";
                dgvData.Columns[0].Width = 80; // Slightly wider
                dgvData.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvData.Columns[0].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                dgvData.Columns[0].ReadOnly = true;
                dgvData.Columns[0].Frozen = true;
                dgvData.Columns[0].MinimumWidth = 80;
                dgvData.Columns[0].Resizable = DataGridViewTriState.False;
                dgvData.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            // Ensure Checkbox column properties
            if (dgvData.Columns.Count > 1)
            {
                dgvData.Columns[1].HeaderText = "☑";
                dgvData.Columns[1].Width = 60; // Slightly wider
                dgvData.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvData.Columns[1].ReadOnly = false;
                dgvData.Columns[1].Frozen = true;
                dgvData.Columns[1].MinimumWidth = 60;
                dgvData.Columns[1].Resizable = DataGridViewTriState.False;
                dgvData.Columns[1].CellTemplate = new DataGridViewCheckBoxCell();
                dgvData.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void DgvData_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                // Only handle checkbox column header click (column index 1)
                if (e.ColumnIndex == 1 && dgvData.Rows.Count > 0)
                {
                    // Count how many are currently checked
                    int checkedCount = 0;
                    foreach (DataGridViewRow row in dgvData.Rows)
                    {
                        if (row.Cells.Count > 1 && row.Cells[1].Value is bool isChecked && isChecked)
                        {
                            checkedCount++;
                        }
                    }

                    // If more than half are checked, uncheck all; otherwise check all
                    bool shouldCheckAll = checkedCount < dgvData.Rows.Count / 2;

                    // Apply to all rows
                    foreach (DataGridViewRow row in dgvData.Rows)
                    {
                        if (row.Cells.Count > 1)
                        {
                            row.Cells[1].Value = shouldCheckAll;
                        }
                    }

                    // Update status bar immediately after header checkbox change
                    UpdateStatusBar();
                }
            }
            catch (Exception ex)
            {
                // Log error but don't show message box to avoid UI freezing
                System.Diagnostics.Debug.WriteLine($"Error in header click: {ex.Message}");
            }
        }

        private void DgvData_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Handle checkbox column clicks (column index 1)
                if (e.ColumnIndex == 1 && e.RowIndex >= 0)
                {
                    // Toggle the checkbox value
                    if (dgvData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value is bool currentValue)
                    {
                        dgvData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = !currentValue;
                    }
                    else
                    {
                        dgvData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = true;
                    }

                    // Update status bar immediately after checkbox change
                    UpdateStatusBar();
                }
            }
            catch (Exception ex)
            {
                // Log error but don't show message box to avoid UI freezing
                System.Diagnostics.Debug.WriteLine($"Error in cell click: {ex.Message}");
            }
        }

        private void ShowProgress(bool show, string message = "")
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ShowProgress(show, message)));
                return;
            }

            progressBar.Visible = show;
            lblProgress.Visible = show;
            lblProgress.Text = message;

            if (show)
            {
                progressBar.Style = ProgressBarStyle.Marquee;
                progressBar.MarqueeAnimationSpeed = 30;
            }
        }

        private void UpdateProgress(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateProgress(message)));
                return;
            }

            if (lblProgress != null)
            {
                lblProgress.Text = message;
            }
        }

        private (DataTable dt, int lineCount, int maxColumns) ProcessDataWithProgress(string inputText, string delimiter, CancellationToken cancellationToken)
        {
            delimiter = string.IsNullOrWhiteSpace(delimiter) ? "|" : delimiter;
            string[] lines = inputText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            DataTable dt = new DataTable();

            // Add STT column first
            dt.Columns.Add("STT", typeof(int));

            // Add Checkbox column
            dt.Columns.Add("☑", typeof(bool));

            // Find maximum number of columns (optimized for large datasets)
            int maxColumns = 0;
            int batchSize = 500; // Smaller batch size for better responsiveness

            for (int i = 0; i < lines.Length; i += batchSize)
            {
                cancellationToken.ThrowIfCancellationRequested();

                int endIndex = Math.Min(i + batchSize, lines.Length);
                for (int j = i; j < endIndex; j++)
                {
                    string[] parts = lines[j].Split(new[] { delimiter }, StringSplitOptions.None);
                    maxColumns = Math.Max(maxColumns, parts.Length);
                }

                // Update progress every batch
                if (i % (batchSize * 5) == 0) // Update every 5 batches
                {
                    UpdateProgress($"Đang phân tích dữ liệu... {i}/{lines.Length}");
                }
            }

            // Create data columns
            for (int i = 1; i <= maxColumns; i++)
            {
                dt.Columns.Add($"Column{i}", typeof(string));
            }

            // Add rows in smaller batches for better responsiveness
            int rowNumber = 1;
            batchSize = 200; // Even smaller batch size for row processing

            for (int i = 0; i < lines.Length; i += batchSize)
            {
                cancellationToken.ThrowIfCancellationRequested();

                int endIndex = Math.Min(i + batchSize, lines.Length);
                for (int j = i; j < endIndex; j++)
                {
                    string[] parts = lines[j].Split(new[] { delimiter }, StringSplitOptions.None);
                    DataRow row = dt.NewRow();

                    // Set STT
                    row[0] = rowNumber++;

                    // Set Checkbox (default false)
                    row[1] = false;

                    // Set data columns (starting from index 2)
                    for (int k = 0; k < parts.Length; k++)
                    {
                        row[k + 2] = parts[k].Trim();
                    }

                    // Fill empty cells
                    for (int k = parts.Length; k < maxColumns; k++)
                    {
                        row[k + 2] = "";
                    }

                    dt.Rows.Add(row);
                }

                // Update progress every batch
                if (i % (batchSize * 3) == 0) // Update every 3 batches
                {
                    UpdateProgress($"Đang xử lý dữ liệu... {i}/{lines.Length}");
                }
            }

            return (dt, lines.Length, maxColumns);
        }

        private (DataTable dt, int lineCount, int maxColumns) ProcessDataAsync(string inputText, string delimiter, CancellationToken cancellationToken)
        {
            delimiter = string.IsNullOrWhiteSpace(delimiter) ? "|" : delimiter;
            string[] lines = inputText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            DataTable dt = new DataTable();

            // Add STT column first
            dt.Columns.Add("STT", typeof(int));

            // Add Checkbox column
            dt.Columns.Add("☑", typeof(bool));

            // Find maximum number of columns (optimized for large datasets)
            int maxColumns = 0;
            int batchSize = 1000; // Process in batches to avoid blocking

            for (int i = 0; i < lines.Length; i += batchSize)
            {
                cancellationToken.ThrowIfCancellationRequested();

                int endIndex = Math.Min(i + batchSize, lines.Length);
                for (int j = i; j < endIndex; j++)
                {
                    string[] parts = lines[j].Split(new[] { delimiter }, StringSplitOptions.None);
                    maxColumns = Math.Max(maxColumns, parts.Length);
                }
            }

            // Create data columns
            for (int i = 1; i <= maxColumns; i++)
            {
                dt.Columns.Add($"Column{i}", typeof(string));
            }

            // Add rows in batches for better performance
            int rowNumber = 1;
            for (int i = 0; i < lines.Length; i += batchSize)
            {
                cancellationToken.ThrowIfCancellationRequested();

                int endIndex = Math.Min(i + batchSize, lines.Length);
                for (int j = i; j < endIndex; j++)
                {
                    string[] parts = lines[j].Split(new[] { delimiter }, StringSplitOptions.None);
                    DataRow row = dt.NewRow();

                    // Set STT
                    row[0] = rowNumber++;

                    // Set Checkbox (default false)
                    row[1] = false;

                    // Set data columns (starting from index 2)
                    for (int k = 0; k < parts.Length; k++)
                    {
                        row[k + 2] = parts[k].Trim();
                    }

                    // Fill empty cells
                    for (int k = parts.Length; k < maxColumns; k++)
                    {
                        row[k + 2] = "";
                    }

                    dt.Rows.Add(row);
                }
            }

            return (dt, lines.Length, maxColumns);
        }

        private async Task UpdateUIAsync((DataTable dt, int lineCount, int maxColumns) result)
        {
            // Update UI on main thread without blocking
            if (InvokeRequired)
            {
                await Task.Run(() =>
                {
                    Invoke(new Action(() => UpdateUI(result)));
                });
            }
            else
            {
                UpdateUI(result);
            }
        }

        private async Task UpdateUIAppendAsync((DataTable dt, int lineCount, int maxColumns) result)
        {
            // Update UI on main thread without blocking - append data
            if (InvokeRequired)
            {
                await Task.Run(() =>
                {
                    Invoke(new Action(() => UpdateUIAppend(result)));
                });
            }
            else
            {
                UpdateUIAppend(result);
            }
        }

        private void UpdateUI((DataTable dt, int lineCount, int maxColumns) result)
        {
            // Suspend layout for better performance
            dgvData.SuspendLayout();

            try
            {
                dgvData.DataSource = result.dt;

                // Set STT column properties after DataSource is set
                if (dgvData.Columns.Count > 0)
                {
                    dgvData.Columns[0].Width = 80; // STT column width
                    dgvData.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvData.Columns[0].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                    dgvData.Columns[0].HeaderText = "STT";
                    dgvData.Columns[0].ReadOnly = true;
                    dgvData.Columns[0].Frozen = true; // Freeze STT column
                    dgvData.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                // Set Checkbox column properties
                if (dgvData.Columns.Count > 1)
                {
                    dgvData.Columns[1].Width = 60; // Checkbox column width
                    dgvData.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvData.Columns[1].ReadOnly = false;
                    dgvData.Columns[1].Frozen = true; // Freeze checkbox column
                    dgvData.Columns[1].CellTemplate = new DataGridViewCheckBoxCell();
                    dgvData.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                // Set data columns properties (skip STT and Checkbox columns)
                for (int i = 2; i < dgvData.Columns.Count; i++)
                {
                    dgvData.Columns[i].Width = 120; // Fixed width for better performance
                    dgvData.Columns[i].DefaultCellStyle.Font = new Font("Segoe UI", 9);
                    dgvData.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                }

                // Ensure STT column is properly set after all operations
                EnsureSTTColumn();

                // Update stats
                UpdateStats(result.lineCount, result.maxColumns);

                // Resize controls to fit current window size
                ResizeControls();

                // Update status bar
                UpdateStatusBar();
            }
            finally
            {
                // Resume layout
                dgvData.ResumeLayout();
            }
        }

        private void UpdateUIAppend((DataTable dt, int lineCount, int maxColumns) result)
        {
            // Suspend layout for better performance
            dgvData.SuspendLayout();

            try
            {
                // If no existing data, just set the new data
                if (dgvData.DataSource == null)
                {
                    dgvData.DataSource = result.dt;
                }
                else
                {
                    // Get existing DataTable
                    var existingTable = (DataTable)dgvData.DataSource;

                    // Get starting STT number
                    int startSTT = existingTable.Rows.Count + 1;

                    // Add new rows to existing table
                    foreach (DataRow newRow in result.dt.Rows)
                    {
                        var newDataRow = existingTable.NewRow();

                        // Copy all columns from new row to existing table
                        for (int i = 0; i < newRow.ItemArray.Length; i++)
                        {
                            if (i < existingTable.Columns.Count)
                            {
                                newDataRow[i] = newRow[i];
                            }
                        }

                        // Update STT column (first column)
                        if (existingTable.Columns.Count > 0)
                        {
                            newDataRow[0] = startSTT++;
                        }

                        existingTable.Rows.Add(newDataRow);
                    }

                    // Refresh the DataGridView
                    dgvData.DataSource = null;
                    dgvData.DataSource = existingTable;
                }

                // Set STT column properties after DataSource is set
                if (dgvData.Columns.Count > 0)
                {
                    dgvData.Columns[0].Width = 80; // STT column width
                    dgvData.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvData.Columns[0].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                    dgvData.Columns[0].HeaderText = "STT";
                    dgvData.Columns[0].ReadOnly = true;
                    dgvData.Columns[0].Frozen = true; // Freeze STT column
                    dgvData.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                // Set Checkbox column properties
                if (dgvData.Columns.Count > 1)
                {
                    dgvData.Columns[1].Width = 60; // Checkbox column width
                    dgvData.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvData.Columns[1].ReadOnly = false;
                    dgvData.Columns[1].Frozen = true; // Freeze checkbox column
                    dgvData.Columns[1].CellTemplate = new DataGridViewCheckBoxCell();
                    dgvData.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                // Set data columns properties (skip STT and Checkbox columns)
                for (int i = 2; i < dgvData.Columns.Count; i++)
                {
                    dgvData.Columns[i].Width = 120; // Fixed width for better performance
                    dgvData.Columns[i].DefaultCellStyle.Font = new Font("Segoe UI", 9);
                    dgvData.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                }

                // Ensure STT column is properly set after all operations
                EnsureSTTColumn();

                // Update stats with total count
                int totalRows = dgvData.Rows.Count;
                UpdateStats(totalRows, result.maxColumns);

                // Resize controls to fit current window size
                ResizeControls();

                // Update status bar
                UpdateStatusBar();
            }
            finally
            {
                // Resume layout
                dgvData.ResumeLayout();
            }
        }

        private void DeleteSelectedRows()
        {
            try
            {
                if (dgvData.SelectedCells.Count > 0)
                {
                    // Get unique row indices from selected cells
                    var selectedRowIndices = dgvData.SelectedCells.Cast<DataGridViewCell>()
                        .Select(cell => cell.RowIndex)
                        .Distinct()
                        .OrderByDescending(index => index)
                        .ToList();

                    if (selectedRowIndices.Count > 0)
                    {
                        DialogResult result = MessageBox.Show($"Bạn có chắc muốn xóa {selectedRowIndices.Count} dòng đã chọn?",
                            "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            // Remove selected rows (from bottom to top to avoid index issues)
                            foreach (int rowIndex in selectedRowIndices)
                            {
                                dgvData.Rows.RemoveAt(rowIndex);
                            }
                            MessageBox.Show($"Đã xóa {selectedRowIndices.Count} dòng!", "Thành công",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Update status bar after deletion
                            UpdateStatusBar();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa dòng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // New event handlers for context menu
        private async void PasteFromClipboardKeepOld_Click(object? sender, EventArgs e)
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    string clipboardText = Clipboard.GetText();

                    // Show progress
                    ShowProgress(true, "Đang xử lý dữ liệu từ clipboard (giữ lại data cũ)...");

                    // Process data asynchronously without clearing old data
                    await ProcessDataAsyncKeepOld(clipboardText, txtDelimiter.Text);

                    // Hide progress
                    ShowProgress(false, "");

                }
            }
            catch (Exception ex)
            {
                ShowProgress(false, "");
                MessageBox.Show($"Lỗi khi paste từ clipboard: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ImportFileKeepOld_Click(object? sender, EventArgs e)
        {
            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    string content = await File.ReadAllTextAsync(filePath);

                    // Show progress
                    ShowProgress(true, "Đang import file (giữ lại data cũ)...");

                    // Process data asynchronously without clearing old data
                    await ProcessDataAsyncKeepOld(content, txtDelimiter.Text);

                    // Hide progress
                    ShowProgress(false, "");

                }
            }
            catch (Exception ex)
            {
                ShowProgress(false, "");
                MessageBox.Show($"Lỗi khi import file: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SelectHighlighted_Click(object? sender, EventArgs e)
        {
            try
            {
                if (dgvData.SelectedCells.Count > 0 && dgvData.Columns.Count > 1)
                {
                    // First, uncheck all rows
                    for (int i = 0; i < dgvData.Rows.Count; i++)
                    {
                        if (dgvData.Rows[i].Cells.Count > 1)
                        {
                            dgvData.Rows[i].Cells[1].Value = false;
                        }
                    }

                    // Get unique row indices from selected cells
                    var selectedRowIndices = dgvData.SelectedCells
                        .Cast<DataGridViewCell>()
                        .Select(cell => cell.RowIndex)
                        .Distinct()
                        .Where(rowIndex => rowIndex >= 0 && rowIndex < dgvData.Rows.Count)
                        .ToList();

                    // Check checkboxes for highlighted rows only
                    foreach (int rowIndex in selectedRowIndices)
                    {
                        if (dgvData.Rows[rowIndex].Cells.Count > 1)
                        {
                            dgvData.Rows[rowIndex].Cells[1].Value = true;
                        }
                    }

                    UpdateStatusBar();
                }
                else
                {
                    MessageBox.Show("Vui lòng bôi đen các dòng cần chọn trước!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chọn dòng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeselectHighlighted_Click(object? sender, EventArgs e)
        {
            try
            {
                if (dgvData.SelectedCells.Count > 0 && dgvData.Columns.Count > 1)
                {
                    // Get unique row indices from selected cells
                    var selectedRowIndices = dgvData.SelectedCells
                        .Cast<DataGridViewCell>()
                        .Select(cell => cell.RowIndex)
                        .Distinct()
                        .Where(rowIndex => rowIndex >= 0 && rowIndex < dgvData.Rows.Count)
                        .ToList();

                    // Uncheck checkboxes for highlighted rows only
                    foreach (int rowIndex in selectedRowIndices)
                    {
                        if (dgvData.Rows[rowIndex].Cells.Count > 1)
                        {
                            dgvData.Rows[rowIndex].Cells[1].Value = false;
                        }
                    }

                    UpdateStatusBar();
                }
                else
                {
                    MessageBox.Show("Vui lòng bôi đen các dòng cần bỏ chọn trước!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi bỏ chọn dòng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteAllData_Click(object? sender, EventArgs e)
        {
            try
            {
                if (dgvData.Rows.Count > 0)
                {
                    dgvData.DataSource = null;
                    UpdateStatusBar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa toàn bộ dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteHighlighted_Click(object? sender, EventArgs e)
        {
            try
            {
                if (dgvData.SelectedCells.Count > 0)
                {
                    // Get unique row indices from selected cells
                    var selectedRowIndices = dgvData.SelectedCells.Cast<DataGridViewCell>()
                        .Select(cell => cell.RowIndex)
                        .Distinct()
                        .OrderByDescending(index => index)
                        .ToList();

                    if (selectedRowIndices.Count > 0)
                    {
                        // Remove selected rows (from bottom to top to avoid index issues)
                        foreach (int rowIndex in selectedRowIndices)
                        {
                            dgvData.Rows.RemoveAt(rowIndex);
                        }

                        // Update status bar after deletion
                        UpdateStatusBar();
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng bôi đen các dòng cần xóa trước!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa dòng bôi đen: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteSelected_Click(object? sender, EventArgs e)
        {
            try
            {
                if (dgvData.Rows.Count > 0 && dgvData.Columns.Count > 1)
                {
                    // Find rows with checked checkboxes
                    var rowsToDelete = new List<int>();
                    for (int i = 0; i < dgvData.Rows.Count; i++)
                    {
                        if (dgvData.Rows[i].Cells.Count > 1 &&
                            dgvData.Rows[i].Cells[1].Value is bool isChecked && isChecked)
                        {
                            rowsToDelete.Add(i);
                        }
                    }

                    if (rowsToDelete.Count > 0)
                    {
                        // Delete rows in reverse order to maintain indices
                        for (int i = rowsToDelete.Count - 1; i >= 0; i--)
                        {
                            dgvData.Rows.RemoveAt(rowsToDelete[i]);
                        }
                        UpdateStatusBar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa dòng tích chọn: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteNotHighlighted_Click(object? sender, EventArgs e)
        {
            try
            {
                if (dgvData.Rows.Count > 0)
                {
                    // Get highlighted row indices
                    var highlightedRowIndices = dgvData.SelectedCells
                        .Cast<DataGridViewCell>()
                        .Select(cell => cell.RowIndex)
                        .Distinct()
                        .ToHashSet();

                    // Find rows that are NOT highlighted
                    var rowsToDelete = new List<int>();
                    for (int i = 0; i < dgvData.Rows.Count; i++)
                    {
                        if (!highlightedRowIndices.Contains(i))
                        {
                            rowsToDelete.Add(i);
                        }
                    }

                    if (rowsToDelete.Count > 0)
                    {
                        // Delete rows in reverse order to maintain indices
                        for (int i = rowsToDelete.Count - 1; i >= 0; i--)
                        {
                            dgvData.Rows.RemoveAt(rowsToDelete[i]);
                        }
                        UpdateStatusBar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa dòng không bôi đen: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteNotSelected_Click(object? sender, EventArgs e)
        {
            try
            {
                if (dgvData.Rows.Count > 0 && dgvData.Columns.Count > 1)
                {
                    // Find rows with unchecked checkboxes
                    var rowsToDelete = new List<int>();
                    for (int i = 0; i < dgvData.Rows.Count; i++)
                    {
                        if (dgvData.Rows[i].Cells.Count > 1)
                        {
                            if (dgvData.Rows[i].Cells[1].Value is bool isChecked && !isChecked)
                            {
                                rowsToDelete.Add(i);
                            }
                            else if (dgvData.Rows[i].Cells[1].Value == null)
                            {
                                rowsToDelete.Add(i);
                            }
                        }
                    }

                    if (rowsToDelete.Count > 0)
                    {
                        // Delete rows in reverse order to maintain indices
                        for (int i = rowsToDelete.Count - 1; i >= 0; i--)
                        {
                            dgvData.Rows.RemoveAt(rowsToDelete[i]);
                        }
                        UpdateStatusBar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa dòng không tích chọn: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Helper method for keeping old data
        private async Task ProcessDataAsyncKeepOld(string inputText, string delimiter)
        {
            try
            {
                // Show progress
                ShowProgress(true, "Đang xử lý dữ liệu (giữ lại data cũ)...");

                // Create cancellation token
                cancellationTokenSource?.Cancel();
                cancellationTokenSource = new CancellationTokenSource();

                // Process data in background thread
                var result = await Task.Run(() => ProcessDataWithProgress(inputText, delimiter, cancellationTokenSource.Token));

                // Update UI on main thread - append data instead of replacing
                await UpdateUIAppendAsync(result);
            }
            catch (OperationCanceledException)
            {
                // User cancelled, do nothing
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xử lý dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ShowProgress(false, "");
            }
        }

        private void CopySelectedData()
        {
            try
            {
                if (dgvData.SelectedCells.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn ô cần copy!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                StringBuilder sb = new StringBuilder();

                // Get selected cells grouped by row
                var selectedCellsByRow = dgvData.SelectedCells.Cast<DataGridViewCell>()
                    .GroupBy(cell => cell.RowIndex)
                    .OrderBy(group => group.Key);

                // Copy selected data
                foreach (var rowGroup in selectedCellsByRow)
                {
                    var cellsInRow = rowGroup.OrderBy(cell => cell.ColumnIndex).ToList();

                    for (int i = 0; i < cellsInRow.Count; i++)
                    {
                        sb.Append(cellsInRow[i].Value?.ToString() ?? "");
                        if (i < cellsInRow.Count - 1)
                            sb.Append("\t");
                    }
                    sb.AppendLine();
                }

                Clipboard.SetText(sb.ToString());
                MessageBox.Show($"Đã copy {dgvData.SelectedCells.Count} ô vào clipboard!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi copy dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Export functionality event handlers
        private void BtnClearExport_Click(object? sender, EventArgs e)
        {
            txtExportFormat.Clear();
        }

        private void ColumnButton_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is int columnNumber)
            {
                string columnName = $"Column{columnNumber}";

                if (string.IsNullOrEmpty(txtExportFormat.Text))
                {
                    txtExportFormat.Text = columnName + "|";
                }
                else
                {
                    txtExportFormat.Text += columnName + "|";
                }

                // Focus back to textbox
                txtExportFormat.Focus();
                txtExportFormat.SelectionStart = txtExportFormat.Text.Length;
            }
        }

        private void ExportSelectedData()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtExportFormat.Text))
                {
                    MessageBox.Show("Vui lòng nhập định dạng export hoặc chọn cột!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (dgvData.Rows.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu để export!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Parse export format
                string[] columnNames = txtExportFormat.Text.Split('|', StringSplitOptions.RemoveEmptyEntries);
                if (columnNames.Length == 0)
                {
                    MessageBox.Show("Định dạng export không hợp lệ!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Find selected rows (with checked checkboxes)
                var selectedRows = new List<DataGridViewRow>();
                foreach (DataGridViewRow row in dgvData.Rows)
                {
                    if (row.Cells.Count > 1 && row.Cells[1].Value is bool isChecked && isChecked)
                    {
                        selectedRows.Add(row);
                    }
                }

                if (selectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng tích chọn ít nhất một dòng để export!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Build export data
                StringBuilder sb = new StringBuilder();

                foreach (var row in selectedRows)
                {
                    for (int i = 0; i < columnNames.Length; i++)
                    {
                        string columnName = columnNames[i].Trim();

                        // Find column index by name
                        int columnIndex = -1;
                        for (int j = 2; j < dgvData.Columns.Count; j++) // Skip STT and Checkbox columns
                        {
                            if (dgvData.Columns[j].HeaderText.Equals(columnName, StringComparison.OrdinalIgnoreCase))
                            {
                                columnIndex = j;
                                break;
                            }
                        }

                        if (columnIndex >= 0 && columnIndex < row.Cells.Count)
                        {
                            sb.Append(row.Cells[columnIndex].Value?.ToString() ?? "");
                        }

                        if (i < columnNames.Length - 1)
                            sb.Append("|");
                    }
                    sb.AppendLine();
                }

                Clipboard.SetText(sb.ToString());
                MessageBox.Show($"Đã export {selectedRows.Count} dòng đã chọn theo định dạng!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi export dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportSelectedDataToFile()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtExportFormat.Text))
                {
                    MessageBox.Show("Vui lòng nhập định dạng export hoặc chọn cột!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (dgvData.Rows.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu để export!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Parse export format
                string[] columnNames = txtExportFormat.Text.Split('|', StringSplitOptions.RemoveEmptyEntries);
                if (columnNames.Length == 0)
                {
                    MessageBox.Show("Định dạng export không hợp lệ!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Find selected rows (with checked checkboxes)
                var selectedRows = new List<DataGridViewRow>();
                foreach (DataGridViewRow row in dgvData.Rows)
                {
                    if (row.Cells.Count > 1 && row.Cells[1].Value is bool isChecked && isChecked)
                    {
                        selectedRows.Add(row);
                    }
                }

                if (selectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng tích chọn ít nhất một dòng để export!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Build export data
                    StringBuilder sb = new StringBuilder();

                    foreach (var row in selectedRows)
                    {
                        for (int i = 0; i < columnNames.Length; i++)
                        {
                            string columnName = columnNames[i].Trim();

                            // Find column index by name
                            int columnIndex = -1;
                            for (int j = 2; j < dgvData.Columns.Count; j++) // Skip STT and Checkbox columns
                            {
                                if (dgvData.Columns[j].HeaderText.Equals(columnName, StringComparison.OrdinalIgnoreCase))
                                {
                                    columnIndex = j;
                                    break;
                                }
                            }

                            if (columnIndex >= 0 && columnIndex < row.Cells.Count)
                            {
                                sb.Append(row.Cells[columnIndex].Value?.ToString() ?? "");
                            }

                            if (i < columnNames.Length - 1)
                                sb.Append("|");
                        }
                        sb.AppendLine();
                    }

                    File.WriteAllText(saveFileDialog.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show($"Đã export {selectedRows.Count} dòng đã chọn ra file: {Path.GetFileName(saveFileDialog.FileName)}", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi export dữ liệu ra file: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Helper method to create rounded rectangle path
        private System.Drawing.Drawing2D.GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseAllFigures();
            return path;
        }

        private Image CreateEmojiImage(string emoji)
        {
            try
            {
                // Create a larger bitmap for better quality
                Bitmap bitmap = new Bitmap(20, 20);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.Transparent);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                    using (Font font = new Font("Segoe UI Emoji", 14, FontStyle.Regular))
                    {
                        using (Brush brush = new SolidBrush(Color.White))
                        {
                            // Center the emoji in the bitmap
                            SizeF textSize = g.MeasureString(emoji, font);
                            float x = (bitmap.Width - textSize.Width) / 2;
                            float y = (bitmap.Height - textSize.Height) / 2;
                            g.DrawString(emoji, font, brush, x, y);
                        }
                    }
                }
                return bitmap;
            }
            catch
            {
                // Fallback to a simple colored rectangle if emoji fails
                Bitmap bitmap = new Bitmap(20, 20);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.FromArgb(70, 130, 180)); // Nice blue color
                    g.FillEllipse(new SolidBrush(Color.White), 6, 6, 8, 8);
                }
                return bitmap;
            }
        }

        private ToolStripMenuItem CreateWindows11MenuItem(string text, Image? image, EventHandler? clickHandler = null)
        {
            var menuItem = new ToolStripMenuItem(text, image, clickHandler)
            {
                BackColor = Color.FromArgb(45, 45, 48), // VS Code exact background
                ForeColor = Color.FromArgb(212, 212, 212), // VS Code exact text color
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Margin = new Padding(0),
                Padding = new Padding(8, 4, 8, 4), // VS Code exact padding
                AutoSize = true
            };

            // Set submenu colors to match main menu
            menuItem.DropDown.BackColor = Color.FromArgb(45, 45, 48);
            menuItem.DropDown.ForeColor = Color.FromArgb(212, 212, 212);
            menuItem.DropDown.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            // Add simple hover effect
            menuItem.MouseEnter += (s, e) =>
            {
                menuItem.BackColor = Color.FromArgb(68, 68, 68); // VS Code exact hover color
            };
            menuItem.MouseLeave += (s, e) =>
            {
                menuItem.BackColor = Color.FromArgb(45, 45, 48);
            };

            return menuItem;
        }
    }
}

// Custom Rounded Button class
public class RoundedButton : Button
{
    private Color _borderColor = Color.FromArgb(0, 122, 204);
    private int _borderRadius = 8;
    private bool _isHovered = false;
    private bool _isPressed = false;

    public RoundedButton()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
        this.FlatStyle = FlatStyle.Flat;
        this.FlatAppearance.BorderSize = 0;
        this.Cursor = Cursors.Hand;
        this.BackColor = Color.White;
        this.ForeColor = Color.FromArgb(0, 122, 204);
        this.UseVisualStyleBackColor = false;
        this.TabStop = false;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        // Determine colors based on state
        Color backColor = this.BackColor;
        Color borderColor = _borderColor;
        Color textColor = this.ForeColor;

        if (_isPressed)
        {
            backColor = Color.FromArgb(200, 230, 255);
            borderColor = Color.FromArgb(0, 100, 180);
        }
        else if (_isHovered)
        {
            backColor = Color.FromArgb(240, 248, 255);
            borderColor = Color.FromArgb(0, 140, 220);
        }

        // Create rounded rectangle path
        Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
        using (System.Drawing.Drawing2D.GraphicsPath path = GetRoundedRectanglePath(rect, _borderRadius))
        {
            // Fill background
            using (SolidBrush brush = new SolidBrush(backColor))
            {
                g.FillPath(brush, path);
            }

            // Draw border
            using (Pen pen = new Pen(borderColor, 1))
            {
                g.DrawPath(pen, path);
            }
        }

        // Draw text
        TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
        TextRenderer.DrawText(g, this.Text, this.Font, rect, textColor, flags);
    }

    private System.Drawing.Drawing2D.GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
    {
        var path = new System.Drawing.Drawing2D.GraphicsPath();
        path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
        path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
        path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
        path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
        path.CloseAllFigures();
        return path;
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        _isHovered = true;
        this.Invalidate();
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        _isHovered = false;
        this.Invalidate();
        base.OnMouseLeave(e);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            _isPressed = true;
            this.Invalidate();
        }
        base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        _isPressed = false;
        this.Invalidate();
        base.OnMouseUp(e);
    }

    protected override void OnPaintBackground(PaintEventArgs pevent)
    {
        // Do nothing to prevent default background painting
    }

    protected override void WndProc(ref Message m)
    {
        // Remove focus rectangle
        if (m.Msg == 0x0007) // WM_SETFOCUS
        {
            return;
        }
        base.WndProc(ref m);
    }
}
