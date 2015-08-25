﻿using System;
using System.IO;
using System.Linq;
using System.Windows;
using Shapeshifter.Core.Data;
using Shapeshifter.Core.Factories.Interfaces;
using Shapeshifter.UserInterface.WindowsDesktop.Factories.Interfaces;
using Shapeshifter.UserInterface.WindowsDesktop.Services.Interfaces;
using Shapeshifter.Core.Data.Interfaces;
using Shapeshifter.UserInterface.WindowsDesktop.Controls.Clipboard.Factories.Interfaces;
using Shapeshifter.UserInterface.WindowsDesktop.Controls.Clipboard.Interfaces;
using System.Runtime.InteropServices;

namespace Shapeshifter.UserInterface.WindowsDesktop.Factories
{
    class FileClipboardDataControlFactory : IFileClipboardDataControlFactory
    {
        readonly IDataSourceService dataSourceService;
        readonly IFileIconService fileIconService;

        readonly IClipboardControlFactory<IClipboardFileData, IClipboardFileDataControl> clipboardFileControlFactory;
        readonly IClipboardControlFactory<IClipboardFileCollectionData, IClipboardFileCollectionDataControl> clipboardFileCollectionControlFactory;

        public FileClipboardDataControlFactory(
            IDataSourceService dataSourceService, 
            IFileIconService fileIconService,
            IClipboardControlFactory<IClipboardFileData, IClipboardFileDataControl> clipboardFileControlFactory,
            IClipboardControlFactory<IClipboardFileCollectionData, IClipboardFileCollectionDataControl> clipboardFileCollectionControlFactory)
        {
            this.dataSourceService = dataSourceService;
            this.fileIconService = fileIconService;
            this.clipboardFileControlFactory = clipboardFileControlFactory;
            this.clipboardFileCollectionControlFactory = clipboardFileCollectionControlFactory;
        }

        public IClipboardControl BuildControl(IClipboardData clipboardData)
        {
            if(clipboardData is IClipboardFileCollectionData)
            {
                return clipboardFileCollectionControlFactory.CreateControl((IClipboardFileCollectionData)clipboardData);
            }
            else if(clipboardData is IClipboardFileData)
            {
                return clipboardFileControlFactory.CreateControl((IClipboardFileData)clipboardData);
            }
            else
            {
                throw new ArgumentException("Unknown clipboard data type.", nameof(clipboardData));
            }
        }

        public IClipboardData BuildData(string format, byte[] rawData)
        {
            if (!CanBuildData(format))
            {
                throw new ArgumentException("Can't construct data from this format.", nameof(format));
            }

            //var files = rawData;
            var files = new string[0];
            return ConstructDataFromFiles(files);
        }

        IClipboardData ConstructDataFromFiles(string[] files)
        {
            if (files.Length == 1)
            {
                return ConstructClipboardFileData(files[0]);
            }
            else
            {
                return ConstructClipboardFileCollectionData(files);
            }
        }

        IClipboardData ConstructClipboardFileCollectionData(string[] files)
        {
            return new ClipboardFileCollectionData(dataSourceService)
            {
                Files = files.Select(ConstructClipboardFileData)
            };
        }

        IClipboardFileData ConstructClipboardFileData(string file)
        {
            return new ClipboardFileData(dataSourceService)
            {
                FileName = Path.GetFileName(file),
                FileIcon = fileIconService.GetIcon(file, false)
            };
        }

        public bool CanBuildControl(IClipboardData data)
        {
            return data is IClipboardFileData || data is IClipboardFileCollectionData;
        }

        public bool CanBuildData(string format)
        {
            return format == DataFormats.FileDrop;
        }
    }
}
