﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using System.Transactions;
using Mehdime.Entity;

using SDDB.Domain.Entities;
using SDDB.Domain.DbContexts;
using SDDB.Domain.Infrastructure;

namespace SDDB.Domain.Services
{
    public class AssemblyLogEntryService
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private IDbContextScopeFactory contextScopeFac;

        //Constructors---------------------------------------------------------------------------------------------------------//

        public AssemblyLogEntryService(IDbContextScopeFactory contextScopeFac)
        {
            this.contextScopeFac = contextScopeFac;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //get all 
        public virtual async Task<List<AssemblyLogEntry>> GetAsync(string userId, bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.AssemblyLogEntrys
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive_bl == getActive)
                    .Include(x => x.AssemblyDb).Include(x => x.EnteredByPerson).Include(x => x.AssemblyStatus).Include(x => x.AssignedToProject)
                    .Include(x => x.AssignedToLocation)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records) { record.FillRelatedIfNull(); }

                return records;
            }
        }

        //get by ids
        public virtual async Task<List<AssemblyLogEntry>> GetAsync(string userId, string[] ids, bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");
            if (ids == null || ids.Length == 0) throw new ArgumentNullException("ids");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.AssemblyLogEntrys
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive_bl == getActive && ids.Contains(x.Id))
                    .Include(x => x.AssemblyDb).Include(x => x.EnteredByPerson).Include(x => x.AssemblyStatus).Include(x => x.AssignedToProject)
                    .Include(x => x.AssignedToLocation)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records) { record.FillRelatedIfNull(); }

                return records;
            }
        }
                
        //get by projectIds and componentIds
        public virtual async Task<List<AssemblyLogEntry>> GetByAltIdsAsync(string userId,
            string[] projectIds = null, string[] assemblyIds = null, string[] personIds = null,
            DateTime? startDate = null, DateTime? endDate = null, bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                projectIds = projectIds ?? new string[] { }; assemblyIds = assemblyIds ?? new string[] { }; personIds = personIds ?? new string[] { };

                var records = await dbContext.AssemblyLogEntrys
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive_bl == getActive &&
                        (projectIds.Count() == 0 || projectIds.Contains(x.AssignedToProject_Id)) &&
                        (assemblyIds.Count() == 0 || assemblyIds.Contains(x.AssemblyDb_Id)) &&
                        (personIds.Count() == 0 || personIds.Contains(x.EnteredByPerson_Id)) &&
                        (startDate == null || x.LogEntryDateTime >= startDate) &&
                        (endDate == null || x.LogEntryDateTime <= endDate)  )
                    .Include(x => x.AssemblyDb).Include(x => x.EnteredByPerson).Include(x => x.AssemblyStatus).Include(x => x.AssignedToProject)
                    .Include(x => x.AssignedToLocation)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records) { record.FillRelatedIfNull(); }

                return records;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in []
        public virtual async Task<DBResult> EditAsync(AssemblyLogEntry[] records)
        {
            var errorMessage = "";

            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var record in records)
                    {
                        var dbEntry = await dbContext.AssemblyLogEntrys.FindAsync(record.Id).ConfigureAwait(false);
                        if (dbEntry == null)
                        {
                            record.Id = Guid.NewGuid().ToString();
                            dbContext.AssemblyLogEntrys.Add(record);
                        }
                        else
                        {
                            dbEntry.CopyModifiedProps(record);
                        }
                    }
                    await dbContext.SaveChangesWithRetryAsync().ConfigureAwait(false);
                    trans.Complete();
                }
            }
            if (errorMessage == "") return new DBResult();
            else return new DBResult
            {
                StatusCode = HttpStatusCode.Conflict,
                StatusDescription = "Errors editing records:\n" + errorMessage
            };
        }

        // Delete records by their Ids
        public virtual async Task<DBResult> DeleteAsync(string[] ids)
        {
            var errorMessage = "";

            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var id in ids)
                    {
                        var dbEntry = await dbContext.AssemblyLogEntrys.FindAsync(id).ConfigureAwait(false);
                        if (dbEntry != null)
                        {
                            dbEntry.IsActive_bl = false;
                        }
                        else
                        {
                            errorMessage += string.Format("Record with Id={0} not found\n", id);
                        }
                    }
                    await dbContext.SaveChangesWithRetryAsync().ConfigureAwait(false);
                    trans.Complete();
                }
            }
            if (errorMessage == "") return new DBResult();
            else return new DBResult
            {
                StatusCode = HttpStatusCode.Conflict,
                StatusDescription = "Errors deleting records:\n" + errorMessage
            };
        }


        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        #endregion
    }
}
