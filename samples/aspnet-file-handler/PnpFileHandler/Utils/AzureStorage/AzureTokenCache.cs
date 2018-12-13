/*
 * PnP File Handler - Sample Code
 * Copyright (c) Microsoft Corporation
 * All rights reserved. 
 * 
 * MIT License
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the ""Software""), to deal in 
 * the Software without restriction, including without limitation the rights to use, 
 * copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
 * Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 * PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

namespace PnpFileHandler.Utils
{
    using System;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Microsoft.WindowsAzure.Storage.Table;

    public class AzureTableTokenCache : TokenCache
    {
        public string User { get; set; }

        private AzureTableContext tables = new AzureTableContext();
        TokenCacheEntity CachedEntity;

        public AzureTableTokenCache(string user)
        {
            this.User = user;
            this.AfterAccess = AfterAccessNotification;
            this.BeforeAccess = BeforeAccessNotification;
            this.BeforeWrite = BeforeWriteNotification;

            CachedEntity = LoadPersistedCacheEntry();
            this.Deserialize((CachedEntity == null) ? null : CachedEntity.CacheBits);
        }

        public override void Clear()
        {
            base.Clear();

            var entry = LoadPersistedCacheEntry();
            if (null != entry)
            {
                TableOperation delete = TableOperation.Delete(entry);
                tables.UserTokenCacheTable.Execute(delete);
            }
            CachedEntity = null;
        }

        private TokenCacheEntity LoadPersistedCacheEntry()
        {
            System.Diagnostics.Debug.WriteLine($"LoadPersistedCacheEntry for {User}");

            TableOperation retrieve = TableOperation.Retrieve<TokenCacheEntity>(TokenCacheEntity.PartitionKeyValue, User);
            TableResult results = tables.UserTokenCacheTable.Execute(retrieve);
            var persistedEntry = (TokenCacheEntity)results.Result;
            return persistedEntry;
        }

        private void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            System.Diagnostics.Debug.WriteLine($"BeforeAccessNotification for {User}");

            // Look up the persisted entry
            var persistedEntry = LoadPersistedCacheEntry();

            if (CachedEntity == null)
            {
                // first time access
                CachedEntity = persistedEntry;
                System.Diagnostics.Debug.WriteLine($"BeforeAccessNotification for {User} - first time access");
            }
            else {
                // if the in-memory copy is older than the persistent copy
                if (persistedEntry != null && persistedEntry.LastWrite > CachedEntity.LastWrite)
                {
                //// read from from storage, update in-memory copy
                CachedEntity = persistedEntry;
                    System.Diagnostics.Debug.WriteLine($"BeforeAccessNotification for {User} - update in-memory cache");
                }
            }

            if (null != CachedEntity)
            {
                System.Diagnostics.Debug.WriteLine($"BeforeAccessNotification for {User} - Deserialize cached entity");
                this.Deserialize(CachedEntity.CacheBits);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"BeforeAccessNotification for {User} - No cached entry exists");
                this.Deserialize(null);
            }
        }

        private void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            System.Diagnostics.Debug.WriteLine($"AfterAccessNotification for {User}");

            if (this.HasStateChanged)
            {
                if (CachedEntity == null)
                {
                    CachedEntity = new TokenCacheEntity();
                }
                CachedEntity.RowKey = User;
                CachedEntity.CacheBits = this.Serialize();
                CachedEntity.LastWrite = DateTime.Now;

                TableOperation insert = TableOperation.InsertOrReplace(CachedEntity);
                tables.UserTokenCacheTable.Execute(insert);
                this.HasStateChanged = false;

                System.Diagnostics.Debug.WriteLine($"Wrote value to persistent cache for {User}");
            }
        }

        private void BeforeWriteNotification(TokenCacheNotificationArgs args)
        {
            System.Diagnostics.Debug.WriteLine($"BeforeWriteNotification for {User}");
        }

    }
}