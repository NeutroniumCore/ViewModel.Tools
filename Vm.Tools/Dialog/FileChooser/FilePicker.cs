﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Vm.Tools.Dialog.FileChooser
{
    public class FilePicker : IFilePicker
    {
        public string Title { get; set; }

        public string Directory { get; set; }
        public string ExtensionDescription { get; set; }
        public string[] Extensions { get; set; }

        public Task<string> Choose()
        {
            var tcs = new TaskCompletionSource<string>();
            Task.Run(() => ShowDialog(tcs));
            return tcs.Task;
        }

        private void ShowDialog(TaskCompletionSource<string> tcs)
        {
            var fileDialog = new OpenFileDialog
            {
                InitialDirectory = Directory,
                Title = Title
            };

            if (Extensions?.Length > 0)
            {
                fileDialog.DefaultExt = Extensions[0];
                var files = String.Join("; ", Extensions.Select(ext => $"*{ext}"));
                fileDialog.Filter = $"{ExtensionDescription} ({files})|{files}";
            }
            CancelEventHandler eventHandler = (o, e) => FileOk(o as OpenFileDialog, e, tcs);
            fileDialog.FileOk += eventHandler;
            fileDialog.ShowDialog();
            fileDialog.FileOk -= eventHandler;
            tcs.TrySetResult(null);
        }

        private void FileOk(OpenFileDialog dia, CancelEventArgs e, TaskCompletionSource<string> tcs)
        {
            tcs.TrySetResult(dia.FileName);
        }
    }
}
