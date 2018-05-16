﻿namespace Shapeshifter.WindowsDesktop.Controls.Clipboard.Designer.Services
{
    using Controls.Designer.Services;
    using Data.Interfaces;
    using Facades;
    using Interfaces;
    using Properties;
    using WindowsDesktop.Services.Clipboard.Interfaces;

    class DesignerFileDataSourceService
        : IDataSourceService,
          IDesignerService
    {
        readonly IDesignerImageConverterService designerImageConverterService;

        public DesignerFileDataSourceService(
            IDesignerImageConverterService designerImageConverterService)
        {
            this.designerImageConverterService = designerImageConverterService;
        }

        public IDataSource GetDataSource()
        {
            return new DesignerDataSourceFacade(designerImageConverterService)
            {
                Title = "My pictures",
                IconLarge = Resources.FileDataSourceIcon,
                IconSmall = Resources.FileDataSourceIcon
            };
        }
    }
}