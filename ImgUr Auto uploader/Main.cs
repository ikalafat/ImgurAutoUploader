using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImgurAutoUploader
{
    public class Main : ApplicationContext
    {
        public NotifyIcon trayIcon;
        KeyboardHook hook = new KeyboardHook();
        PrintScreen printscreen = new PrintScreen();
        ImgurUploader imgur = new ImgurUploader();
        public Main()
        {
            trayIcon = new NotifyIcon()
            {
                Icon = ImgurAutoUploader.Properties.Resources.AppIcon,
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Exit", Exit)}),
                    Visible = true
            };
            trayIcon.Text = "Imgur Auto uploader\nHotkeys: CTRL + PrintScreen";
            hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
            try
            {
                hook.RegisterHotKey(ModifierKeys.Control, Keys.PrintScreen);
            }
            catch (Exception)
            {
                trayIcon.BalloonTipText = "Cannot register hotkey.\nCheck if another application is using CTRL + PrintScreen keys. \nQuitting!";
                trayIcon.BalloonTipIcon = ToolTipIcon.Error;
                trayIcon.BalloonTipTitle = "Error!";
                trayIcon.ShowBalloonTip(400);
                System.Threading.Thread.Sleep(4000);
                trayIcon.Visible = false;
                Environment.Exit(0);            
            }
        }

        void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            byte[] image = printscreen.Take();
            trayIcon.BalloonTipText = "Uploading!";
            trayIcon.BalloonTipIcon = ToolTipIcon.Info;
            trayIcon.BalloonTipTitle = "Info!";
            trayIcon.ShowBalloonTip(100);
            string result = imgur.UploadImage(image);
            if (!result.Contains("Error!"))
            {
                System.Windows.Forms.Clipboard.SetText(result);
                trayIcon.BalloonTipText = "The link has been copied to clipboard!";
                trayIcon.BalloonTipIcon = ToolTipIcon.Info;
                trayIcon.BalloonTipTitle = "Info!";
                trayIcon.ShowBalloonTip(300);
            }
            else
            {
                trayIcon.BalloonTipText = "Error! Something went wrong. \n Check your Internet connection";
                trayIcon.BalloonTipIcon = ToolTipIcon.Error;
                trayIcon.BalloonTipTitle = "Error!";
                trayIcon.ShowBalloonTip(300);
            }
        }

        void Exit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Environment.Exit(0);
        }
    }
}
