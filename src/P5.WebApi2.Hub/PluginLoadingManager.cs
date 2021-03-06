﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;

namespace P5.WebApi2.Hub
{
    public class PluginLoadingManager
    {
        private CompositionContainer _container;

        public PluginLoadingManager(WebApi2HubOptions pluginLoaderConfiguration)
        {
            var catalogue = CreatePluginCatalog(pluginLoaderConfiguration);

            CreateCompositionContainer(catalogue);
        }

        #region Build Step

        private void CreateCompositionContainer(AggregateCatalog catalogue)
        {
            _container = new CompositionContainer(catalogue);
        }

        private AggregateCatalog CreatePluginCatalog(WebApi2HubOptions pluginLoaderConfiguration)
        {
            var pluginsCatalog = new AggregateCatalog();
            var dirs = GetPluginDirectories(pluginLoaderConfiguration.PluginRootPath);

            foreach (var pluginDirectory in dirs)
            {
                pluginsCatalog.Catalogs.Add(new DirectoryCatalog(pluginDirectory));
            }

            return pluginsCatalog;
        }

        /// <summary>
        /// Return a collection of plugin directories to be loaded
        /// </summary>
        /// <param name="baseDirectory"></param>
        /// <returns></returns>
        private string[] GetPluginDirectories(string baseDirectory)
        {
            string[] dirs = {};
            try
            {
                var directories = Directory.EnumerateDirectories(baseDirectory, "*", SearchOption.TopDirectoryOnly);
                return directories.ToArray();
            }
            catch (Exception)
            {
            }
            return dirs;
        }

        #endregion

        public List<IPluginHost> GetPluginHosts()
        {
            var exports = _container
                .GetExports<IPluginHost>();
            var query = from item in exports
                        select item.Value;
            return query.ToList();
        }
    }

}
