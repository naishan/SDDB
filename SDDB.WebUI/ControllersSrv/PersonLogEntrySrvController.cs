﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

using SDDB.Domain.Entities;
using SDDB.Domain.Services;
using SDDB.Domain.Abstract;
using SDDB.WebUI.Infrastructure;
using System.Web;
using System.Collections.Generic;

namespace SDDB.WebUI.ControllersSrv
{
    public class PersonLogEntrySrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private PersonLogEntryService prsLogEntryService;
        private IFileRepoService fileRepoService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public PersonLogEntrySrvController(PersonLogEntryService prsLogEntryService, IFileRepoService fileRepoService)
        {
            this.prsLogEntryService = prsLogEntryService;
            this.fileRepoService = fileRepoService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: /PersonLogEntrySrv/Get
        [DBSrvAuth("PersonLogEntry_View")]
        public async Task<ActionResult> Get(bool getActive = true)
        {
            var data = (await prsLogEntryService.GetAsync(UserId, getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.LogEntryDateTime,
                EnteredByPerson_ = new { x.EnteredByPerson.FirstName, x.EnteredByPerson.LastName, x.EnteredByPerson.Initials },
                PersonActivityType_ = new { x.PersonActivityType.ActivityTypeName },
                x.ManHours, 
                AssignedToProject_ = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectAltName, x.AssignedToProject.ProjectCode },
                AssignedToLocation_ = new { x.AssignedToLocation.LocName, x.AssignedToLocation.LocAltName },
                AssignedToProjectEvent_ = new { x.AssignedToProjectEvent.EventName },  
                x.Comments, x.IsActive_bl,
                x.EnteredByPerson_Id ,x.PersonActivityType_Id, x.AssignedToProject_Id, x.AssignedToLocation_Id, x.AssignedToProjectEvent_Id
            });

            ViewBag.ServiceName = "PersonLogEntryService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return new DBJsonDateTimeISO { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        
        // POST: /PersonLogEntrySrv/GetByIds
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            var data = (await prsLogEntryService.GetAsync(UserId, ids, getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.LogEntryDateTime,
                EnteredByPerson_ = new { x.EnteredByPerson.FirstName, x.EnteredByPerson.LastName, x.EnteredByPerson.Initials },
                PersonActivityType_ = new { x.PersonActivityType.ActivityTypeName },
                x.ManHours, 
                AssignedToProject_ = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectAltName, x.AssignedToProject.ProjectCode },
                AssignedToLocation_ = new { x.AssignedToLocation.LocName, x.AssignedToLocation.LocAltName },
                AssignedToProjectEvent_ = new { x.AssignedToProjectEvent.EventName },  
                x.Comments, x.IsActive_bl,
                x.EnteredByPerson_Id ,x.PersonActivityType_Id, x.AssignedToProject_Id, x.AssignedToLocation_Id, x.AssignedToProjectEvent_Id
            });

            ViewBag.ServiceName = "PersonLogEntryService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return new DBJsonDateTimeISO { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // POST: /PersonLogEntrySrv/GetByAltIds
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_View")]
        public async Task<ActionResult> GetByAltIds(string[] personIds = null, string[] projectIds = null, string[] typeIds = null,
            DateTime? startDate = null, DateTime? endDate = null, bool getActive = true)
        {
            var data = (await prsLogEntryService.GetByAltIdsAsync(UserId, personIds, projectIds, typeIds, startDate, endDate, getActive)
                .ConfigureAwait(false)).Select(x => new {
                x.Id, x.LogEntryDateTime,
                EnteredByPerson_ = new { x.EnteredByPerson.FirstName, x.EnteredByPerson.LastName, x.EnteredByPerson.Initials },
                PersonActivityType_ = new { x.PersonActivityType.ActivityTypeName },
                x.ManHours, 
                AssignedToProject_ = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectAltName, x.AssignedToProject.ProjectCode },
                AssignedToLocation_ = new { x.AssignedToLocation.LocName, x.AssignedToLocation.LocAltName },
                AssignedToProjectEvent_ = new { x.AssignedToProjectEvent.EventName },  
                x.Comments, x.IsActive_bl,
                x.EnteredByPerson_Id ,x.PersonActivityType_Id, x.AssignedToProject_Id, x.AssignedToLocation_Id, x.AssignedToProjectEvent_Id
            });

            ViewBag.ServiceName = "PersonLogEntryService.GetByAltIdsAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return new DBJsonDateTimeISO { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        // GET: /PersonLogEntrySrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            var records = await prsLogEntryService.LookupAsync(UserId, query, getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "PersonLogEntryService.LookupAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            var data = records.OrderBy(x => x.LogEntryDateTime)
                .Select(x => new { id = x.Id, name = x.LogEntryDateTime + " " + x.EnteredByPerson.LastName + " " + x.EnteredByPerson.Initials });

            return new DBJsonDateTimeISO { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // GET: /PersonLogEntrySrv/LookupByProj
        public async Task<ActionResult> LookupByProj(string projectIds = null, string query = "", bool getActive = true)
        {
            string[] projectIdsArray = null;
            if (projectIds != null && projectIds != "") projectIdsArray = projectIds.Split(',');

            var records = await prsLogEntryService.LookupByProjAsync(UserId, projectIdsArray, query, getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "PersonLogEntryService.LookupByProjAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            var data = records.OrderBy(x => x.LogEntryDateTime)
                .Select(x => new { id = x.Id, name = x.LogEntryDateTime + " " + x.EnteredByPerson.LastName + " " + x.EnteredByPerson.Initials });

            return new DBJsonDateTimeISO { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /PersonLogEntrySrv/Edit
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_Edit")]
        public async Task<ActionResult> Edit(PersonLogEntry[] records)
        {
            var serviceResult = await prsLogEntryService.EditAsync(UserId,records).ConfigureAwait(false);

            ViewBag.ServiceName = "PersonLogEntryService.EditAsync"; ViewBag.StatusCode = serviceResult.StatusCode; 
            ViewBag.StatusDescription = serviceResult.StatusDescription;

            if (serviceResult.StatusCode == HttpStatusCode.OK)
            {
                Response.StatusCode = (int)HttpStatusCode.OK; return Json(new {
                    Success = "True", ReturnIds = serviceResult.ReturnIds }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Response.StatusCode = (int)serviceResult.StatusCode;
                return Json(new { Success = "False", responseText = serviceResult.StatusDescription,
                    ReturnIds = serviceResult.ReturnIds }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: /PersonLogEntrySrv/Delete
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            var serviceResult = await prsLogEntryService.DeleteAsync(ids).ConfigureAwait(false);

            ViewBag.ServiceName = "PersonLogEntryService.DeleteAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
            ViewBag.StatusDescription = serviceResult.StatusDescription;

            if (serviceResult.StatusCode == HttpStatusCode.OK)
            {
                Response.StatusCode = (int)HttpStatusCode.OK; return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Response.StatusCode = (int)serviceResult.StatusCode;
                return Json(new { Success = "False", responseText = serviceResult.StatusDescription }, JsonRequestBehavior.AllowGet);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // GET: /PersonLogEntrySrv/GetPrsLogEntryAssys
        [DBSrvAuth("PersonLogEntry_View,Assembly_View")]
        public async Task<ActionResult> GetPrsLogEntryAssys(string logEntryId)
        {
            var data = (await prsLogEntryService.GetPrsLogEntryAssysAsync(logEntryId).ConfigureAwait(false))
                .Select(x => new { x.Id, x.AssyName, x.AssyAltName });

            ViewBag.ServiceName = "PersonLogEntryService.GetPrsLogEntryAssysAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: /PersonLogEntrySrv/GetPrsLogEntryAssysNot
        [DBSrvAuth("PersonLogEntry_View,Assembly_View")]
        public async Task<ActionResult> GetPrsLogEntryAssysNot(string logEntryId, string locId = null)
        {
            var data = (await prsLogEntryService.GetPrsLogEntryAssysNotAsync(logEntryId, locId).ConfigureAwait(false))
                .Select(x => new { x.Id, x.AssyName, x.AssyAltName });

            ViewBag.ServiceName = "PersonLogEntryService.GetPrsLogEntryAssysNotAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // POST: /PersonLogEntrySrv/EditPrsLogEntryAssys
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_Edit,Assembly_Edit")]
        public async Task<ActionResult> EditPrsLogEntryAssys(string[] ids, string[] idsAddRem, bool isAdd)
        {
            var serviceResult = await prsLogEntryService.EditPrsLogEntryAssysAsync(ids, idsAddRem, isAdd).ConfigureAwait(false);

            ViewBag.ServiceName = "PersonLogEntryService.EditPrsLogEntryAssysAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
            ViewBag.StatusDescription = serviceResult.StatusDescription;

            if (serviceResult.StatusCode == HttpStatusCode.OK)
            {
                Response.StatusCode = (int)HttpStatusCode.OK; return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Response.StatusCode = (int)serviceResult.StatusCode;
                return Json(new { Success = "False", responseText = serviceResult.StatusDescription }, JsonRequestBehavior.AllowGet);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // GET: /PersonLogEntrySrv/GetPrsLogEntryPersons
        [DBSrvAuth("PersonLogEntry_View,Person_View")]
        public async Task<ActionResult> GetPrsLogEntryPersons(string logEntryId)
        {
            var data = (await prsLogEntryService.GetPrsLogEntryPersonsAsync(logEntryId).ConfigureAwait(false))
                .Select(x => new { x.Id, x.FirstName, x.LastName, x.Initials });

            ViewBag.ServiceName = "PersonLogEntryService.GetPrsLogEntryPersonsAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: /PersonLogEntrySrv/GetPrsLogEntryPersonsNot
        [DBSrvAuth("PersonLogEntry_View,Person_View")]
        public async Task<ActionResult> GetPrsLogEntryPersonsNot(string logEntryId)
        {
            var data = (await prsLogEntryService.GetPrsLogEntryPersonsNotAsync(UserId, logEntryId).ConfigureAwait(false))
                .Select(x => new { x.Id, x.FirstName, x.LastName, x.Initials });

            ViewBag.ServiceName = "PersonLogEntryService.GetPrsLogEntryPersonsNotAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // POST: /PersonLogEntrySrv/EditPrsLogEntryPersons
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_Edit,Person_Edit")]
        public async Task<ActionResult> EditPrsLogEntryPersons(string[] ids, string[] idsAddRem, bool isAdd)
        {
            var serviceResult = await prsLogEntryService.EditPrsLogEntryPersonsAsync(ids, idsAddRem, isAdd).ConfigureAwait(false);

            ViewBag.ServiceName = "PersonLogEntryService.EditPrsLogEntryPersonsAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
            ViewBag.StatusDescription = serviceResult.StatusDescription;

            if (serviceResult.StatusCode == HttpStatusCode.OK)
            {
                Response.StatusCode = (int)HttpStatusCode.OK; return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Response.StatusCode = (int)serviceResult.StatusCode;
                return Json(new { Success = "False", responseText = serviceResult.StatusDescription }, JsonRequestBehavior.AllowGet);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // GET: /PersonLogEntrySrv/GetFiles
        [DBSrvAuth("PersonLogEntry_View")]
        public async Task<ActionResult> GetFiles(string id)
        {
            var data = (await fileRepoService.GetAsync(id).ConfigureAwait(false));

            ViewBag.ServiceName = "IFileRepoService.Get"; ViewBag.StatusCode = HttpStatusCode.OK;

            return new DBJsonDateTimeISO { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // POST: /PersonLogEntrySrv/DownloadFiles
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_View",false)]
        public async Task<ActionResult> DownloadFiles(long DlToken, string id, string[] names)
        {
            var data = await fileRepoService.DownloadAsync(id, names).ConfigureAwait(false);

            ViewBag.ServiceName = "IFileRepoService.DownloadAsync"; ViewBag.StatusCode = HttpStatusCode.OK;
                        
            var tokenCookie = new HttpCookie("DlToken", DlToken.ToString());
            Response.Cookies.Set(tokenCookie);

            var fileName = (names.Length == 1) ? names[0] : "SDDBFiles.zip";

            if (data != null && data.LongLength != 0) return File(data, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            else
            {
                Response.StatusCode = (int)HttpStatusCode.Gone;
                return Content("File(s) not found.");
            }
        }

        // POST: /PersonLogEntrySrv/UploadFiles
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_Edit")]
        public async Task<ActionResult> UploadFiles(string id)
        {
            var filesList = new List<FileMemStream>();
            foreach (string fileName in Request.Files)
            {
                var fileContent = Request.Files[fileName];
                if (fileContent != null && fileContent.ContentLength > 0)
                {
                    var fileMemStream = new FileMemStream();
                    await fileContent.InputStream.CopyToAsync(fileMemStream).ConfigureAwait(false);
                    fileMemStream.FileName = fileContent.FileName;
                    filesList.Add(fileMemStream);
                }
            }
            
            await fileRepoService.UploadAsync(id, filesList.ToArray()).ConfigureAwait(false);
            var serviceResult = new DBResult();

            ViewBag.ServiceName = "fileRepoService.UploadAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
            ViewBag.StatusDescription = serviceResult.StatusDescription;

            if (serviceResult.StatusCode == HttpStatusCode.OK) return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
            else
            {
                Response.StatusCode = (int)serviceResult.StatusCode;
                return Json(new { Success = "False", responseText = serviceResult.StatusDescription }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: /PersonLogEntrySrv/DeleteFiles
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_Edit")]
        public async Task<ActionResult> DeleteFiles(string id, string[] names)
        {
            await fileRepoService.DeleteAsync(id, names).ConfigureAwait(false);
            var serviceResult = new DBResult();

            ViewBag.ServiceName = "fileRepoService.DeleteAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
            ViewBag.StatusDescription = serviceResult.StatusDescription;

            if (serviceResult.StatusCode == HttpStatusCode.OK) return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
            else
            {
                Response.StatusCode = (int)serviceResult.StatusCode;
                return Json(new { Success = "False", responseText = serviceResult.StatusDescription }, JsonRequestBehavior.AllowGet);
            }
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers


        #endregion
    }
}