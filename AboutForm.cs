using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DataSplitPro
{
    public class AboutForm : Form
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;

        public AboutForm()
        {
            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            try
            {
                int darkMode = 1;
                if (DwmSetWindowAttribute(Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref darkMode, sizeof(int)) != 0)
                {
                    DwmSetWindowAttribute(Handle, DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1, ref darkMode, sizeof(int));
                }
            }
            catch
            {
                // Dark title bar is cosmetic only
            }
        }

        private void InitializeComponent()
        {
            Text = "Giới thiệu - Data Split Pro";
            ClientSize = new Size(460, 420);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(45, 45, 48);

            // Logo (scaled to 48px height, width follows the source aspect ratio)
            var picLogo = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.AutoSize,
                BackColor = Color.Transparent
            };
            try
            {
                if (File.Exists("hasoftware.ico"))
                {
                    using var sourceIcon = new Icon("hasoftware.ico", 256, 256);
                    using var sourceBitmap = sourceIcon.ToBitmap();

                    int targetHeight = 48;
                    int targetWidth = Math.Max(1, (int)Math.Round((double)sourceBitmap.Width * targetHeight / sourceBitmap.Height));

                    var logoBitmap = new Bitmap(targetWidth, targetHeight);
                    using (var g = Graphics.FromImage(logoBitmap))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        g.DrawImage(sourceBitmap, new Rectangle(0, 0, targetWidth, targetHeight));
                    }
                    picLogo.Image = logoBitmap;
                    Icon = new Icon("hasoftware.ico");
                }
            }
            catch
            {
                // Continue without logo
            }

            // App name + version (read from assembly so it always matches the build)
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            string versionText = version != null ? $"v{version.Major}.{version.Minor}" : "";

            var lblTitle = new Label
            {
                Text = "Data Split Pro",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true
            };

            var lblVersion = new Label
            {
                Text = $"Phiên bản {versionText}  •  HASOFTWARE",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(150, 150, 150),
                AutoSize = true
            };

            var lblDescription = new Label
            {
                Text = "Công cụ tách và xử lý dữ liệu văn bản theo cột.\n" +
                       "Paste, import hoặc kéo thả file — dữ liệu được tách thành bảng\n" +
                       "theo ký tự ngăn cách, sẵn sàng lọc, chỉnh sửa và xuất theo ý muốn.",
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.FromArgb(204, 204, 204),
                AutoSize = true
            };

            var lblFeatures = new Label
            {
                Text = "•  Tách dữ liệu theo ký tự ngăn cách tùy chọn\n" +
                       "•  Tìm kiếm && lọc tức thì theo cột hoặc toàn bảng\n" +
                       "•  Xóa trùng lặp khi import\n" +
                       "•  Kéo thả nhiều file để import\n" +
                       "•  Sửa nội dung ô trực tiếp bằng double-click\n" +
                       "•  Export cột tùy chỉnh ra clipboard hoặc file",
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.FromArgb(204, 204, 204),
                AutoSize = true
            };

            var pnlSeparator = new Panel
            {
                Size = new Size(400, 1),
                BackColor = Color.FromArgb(85, 85, 85)
            };

            var lblDev = new Label
            {
                Text = "Developer: Trịnh Hoàng Anh",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true
            };

            var linkTelegram = CreateLink("Telegram", "https://t.me/Hoanganhdev");
            var linkChannel = CreateLink("Channel", "https://t.me/hasoftware");
            var linkGithub = CreateLink("GitHub", "https://github.com/hasoftware");
            var linkFacebook = CreateLink("Facebook", "https://www.facebook.com/100007353125377");

            var lblCopyright = new Label
            {
                Text = $"Copyright © HASOFTWARE {DateTime.Now.Year}",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(120, 120, 120),
                AutoSize = true
            };

            var btnClose = new Button
            {
                Text = "Đóng",
                Font = new Font("Segoe UI", 9),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.OK
            };
            btnClose.FlatAppearance.BorderSize = 0;

            // Layout
            int left = 30;
            picLogo.Location = new Point(left, 25);
            lblTitle.Location = new Point(left + 140, 25);
            lblVersion.Location = new Point(left + 143, 62);
            lblDescription.Location = new Point(left, 95);
            lblFeatures.Location = new Point(left, 160);
            pnlSeparator.Location = new Point(left, 290);
            lblDev.Location = new Point(left, 305);
            linkTelegram.Location = new Point(left, 332);
            linkChannel.Location = new Point(left + 80, 332);
            linkGithub.Location = new Point(left + 160, 332);
            linkFacebook.Location = new Point(left + 235, 332);
            lblCopyright.Location = new Point(left, 375);
            btnClose.Location = new Point(ClientSize.Width - btnClose.Width - 30, 368);

            Controls.AddRange(new Control[]
            {
                picLogo, lblTitle, lblVersion, lblDescription, lblFeatures,
                pnlSeparator, lblDev, linkTelegram, linkChannel, linkGithub, linkFacebook,
                lblCopyright, btnClose
            });

            AcceptButton = btnClose;
            CancelButton = btnClose;
        }

        private static LinkLabel CreateLink(string text, string url)
        {
            var link = new LinkLabel
            {
                Text = text,
                Font = new Font("Segoe UI", 9.5f),
                LinkColor = Color.FromArgb(70, 150, 255),
                ActiveLinkColor = Color.FromArgb(100, 180, 255),
                LinkBehavior = LinkBehavior.HoverUnderline,
                AutoSize = true
            };
            link.LinkClicked += (s, e) =>
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error opening link: {ex.Message}");
                }
            };
            return link;
        }
    }
}
