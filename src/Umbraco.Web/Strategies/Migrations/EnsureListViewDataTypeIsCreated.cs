﻿using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Rdbms;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;
using Umbraco.Core.Persistence.UnitOfWork;
using umbraco.interfaces;

namespace Umbraco.Web.Strategies.Migrations
{
    /// <summary>
    /// Creates the built in list view data types
    /// </summary>
    public class EnsureDefaultListViewDataTypesCreated : ApplicationEventHandler
    {
        /// <summary>
        /// Ensure this is run when not configured
        /// </summary>
        protected override bool ExecuteWhenApplicationNotConfigured
        {
            get { return true; }
        }

        /// <summary>
        /// Ensure this is run when not configured
        /// </summary>
        protected override bool ExecuteWhenDatabaseNotConfigured
        {
            get { return true; }
        }

        /// <summary>
        /// Attach to event on starting
        /// </summary>
        /// <param name="umbracoApplication"></param>
        /// <param name="applicationContext"></param>
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            MigrationRunner.Migrated += MigrationRunner_Migrated;
        }

        void MigrationRunner_Migrated(MigrationRunner sender, Core.Events.MigrationEventArgs e)
        {
            var target720 = new Version(7, 2, 0);

            if (e.ConfiguredVersion <= target720)
            {
                EnsureListViewDataTypeCreated(e);
                
            }
        }

        private void EnsureListViewDataTypeCreated(Core.Events.MigrationEventArgs e)
        {
            var exists = e.MigrationContext.Database.ExecuteScalar<int>("SELECT COUNT(*) FROM umbracoNode WHERE id=1037");
            if (exists > 0) return;

            using (var transaction = e.MigrationContext.Database.GetTransaction())
            {
                //Turn on identity insert if db provider is not mysql
                if (SqlSyntaxContext.SqlSyntaxProvider.SupportsIdentityInsert())
                    e.MigrationContext.Database.Execute(new Sql(string.Format("SET IDENTITY_INSERT {0} ON ", SqlSyntaxContext.SqlSyntaxProvider.GetQuotedTableName("umbracoNode"))));

                e.MigrationContext.Database.Insert("umbracoNode", "id", false, new NodeDto { NodeId = Constants.System.DefaultContentListViewDataTypeId, Trashed = false, ParentId = -1, UserId = 0, Level = 1, Path = "-1,1037", SortOrder = 2, UniqueId = new Guid("C0808DD3-8133-4E4B-8CE8-E2BEA84A96A4"), Text = "List View - Content", NodeObjectType = new Guid(Constants.ObjectTypes.DataType), CreateDate = DateTime.Now });
                e.MigrationContext.Database.Insert("umbracoNode", "id", false, new NodeDto { NodeId = Constants.System.DefaultMediaListViewDataTypeId, Trashed = false, ParentId = -1, UserId = 0, Level = 1, Path = "-1,1038", SortOrder = 2, UniqueId = new Guid("3A0156C4-3B8C-4803-BDC1-6871FAA83FFF"), Text = "List View - Media", NodeObjectType = new Guid(Constants.ObjectTypes.DataType), CreateDate = DateTime.Now });
                e.MigrationContext.Database.Insert("umbracoNode", "id", false, new NodeDto { NodeId = Constants.System.DefaultMembersListViewDataTypeId, Trashed = false, ParentId = -1, UserId = 0, Level = 1, Path = "-1,1039", SortOrder = 2, UniqueId = new Guid("AA2C52A0-CE87-4E65-A47C-7DF09358585D"), Text = "List View - Members", NodeObjectType = new Guid(Constants.ObjectTypes.DataType), CreateDate = DateTime.Now });                                    

                //Turn off identity insert if db provider is not mysql
                if (SqlSyntaxContext.SqlSyntaxProvider.SupportsIdentityInsert())
                    e.MigrationContext.Database.Execute(new Sql(string.Format("SET IDENTITY_INSERT {0} OFF;", SqlSyntaxContext.SqlSyntaxProvider.GetQuotedTableName("umbracoNode"))));

                //Turn on identity insert if db provider is not mysql
                if (SqlSyntaxContext.SqlSyntaxProvider.SupportsIdentityInsert())
                    e.MigrationContext.Database.Execute(new Sql(string.Format("SET IDENTITY_INSERT {0} ON ", SqlSyntaxContext.SqlSyntaxProvider.GetQuotedTableName("cmsDataType"))));

                e.MigrationContext.Database.Insert("cmsDataType", "pk", false, new DataTypeDto { PrimaryKey = 19, DataTypeId = Constants.System.DefaultContentListViewDataTypeId, PropertyEditorAlias = Constants.PropertyEditors.ListViewAlias, DbType = "Nvarchar" });
                e.MigrationContext.Database.Insert("cmsDataType", "pk", false, new DataTypeDto { PrimaryKey = 20, DataTypeId = Constants.System.DefaultMediaListViewDataTypeId, PropertyEditorAlias = Constants.PropertyEditors.ListViewAlias, DbType = "Nvarchar" });
                e.MigrationContext.Database.Insert("cmsDataType", "pk", false, new DataTypeDto { PrimaryKey = 23, DataTypeId = Constants.System.DefaultMembersListViewDataTypeId, PropertyEditorAlias = Constants.PropertyEditors.ListViewAlias, DbType = "Nvarchar" });            

                //Turn off identity insert if db provider is not mysql
                if (SqlSyntaxContext.SqlSyntaxProvider.SupportsIdentityInsert())
                    e.MigrationContext.Database.Execute(new Sql(string.Format("SET IDENTITY_INSERT {0} OFF;", SqlSyntaxContext.SqlSyntaxProvider.GetQuotedTableName("cmsDataType"))));

                transaction.Complete();
            }
        }

    }

}