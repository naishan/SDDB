﻿using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using SDDB.Domain.Abstract;
using SDDB.Domain.Infrastructure;

namespace SDDB.Domain.Entities
{
    [Table("Persons")]
    public class Person : IDbEntity
    {
        //Entity Framework Properties------------------------------------------------------------------------------------------//

        [Key]
        [StringLength(40)]
        [ForeignKey("DBUser")]
        public string Id { get; set; }

        [Required]
        [StringLength(255)]
        public string LastName { get; set; }

        [Required]
        [StringLength(255)]
        public string FirstName { get; set; }

        [Required]
        [DBIsUnique] [Index(IsUnique = true)]
        [StringLength(255)]
        public string Initials { get; set; }

        [StringLength(255)]
        public string Phone { get; set; }

        [StringLength(255)]
        public string PhoneMobile { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        [Column(TypeName = "text")] [StringLength(65535)]
        public string Comments { get; set; }

        [Required]
        [DefaultValue(true)]
        public bool IsActive_bl { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool IsCurrentEmployee_bl { get; set; }

        [StringLength(255)]
        public string EmployeePosition { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool IsSalaried_bl { get; set; }

        [DBIsDateISO] 
        public DateTime? EmployeeStart { get; set; }

        [DBIsDateISO] 
        public DateTime? EmployeeEnd { get; set; }

        [Column(TypeName = "text")] [StringLength(65535)]
        public string EmployeeDetails { get; set; }

        //TSP Column Wireup ----------------------------------------------------------------
        [Column(TypeName = "timestamp")]
        public DateTime TSP
        {
            get { return this.tsp.HasValue ? this.tsp.Value : DateTime.Now; }
            set { tsp = value; }
        }
        [NotMapped]
        private DateTime? tsp;

        //LastSavedByPerson_Id -------------------------------------------------------------
        //[Required(ErrorMessage = "LastSavedByPerson Id field is required")]
        [StringLength(40)]
        [ForeignKey("LastSavedByPerson")]
        public string LastSavedByPerson_Id { get; set; }
        //Navigation Property
        public virtual Person LastSavedByPerson { get; set; }

        //EF Navigation Properties---------------------------------------------------------------------------------------------//

        //one to one
        
        public virtual DBUser DBUser { get; set; }

        //one to many
        
        [InverseProperty("ProjectManager")]
        public virtual ICollection<Project> ProjectsManager { get; set; }

        [InverseProperty("AuthorPerson")]
        public virtual ICollection<Document> DocumentsAuthor { get; set; }

        [InverseProperty("ReviewerPerson")] 
        public virtual ICollection<Document> DocumentsReviewer { get; set; }

        [InverseProperty("ContactPerson")]
        public virtual ICollection<Location> LocationsContact { get; set; }

        [InverseProperty("EnteredByPerson")]
        public virtual ICollection<PersonLogEntry> EnteredPrsLogEntrys { get; set; }

        //many to many

        [InverseProperty("ProjectPersons")]
        public virtual ICollection<Project> PersonProjects { get; set; }

        [InverseProperty("GroupPersons")]
        public virtual ICollection<PersonGroup> PersonGroups { get; set; }

        [InverseProperty("GroupManagers")]
        public virtual ICollection<PersonGroup> ManagedGroups { get; set; }

        [InverseProperty("PrsLogEntryPersons")]
        public virtual ICollection<PersonLogEntry> PersonPrsLogEntrys { get; set; }


        //Non-persistent Properties--------------------------------------------------------------------------------------------//

        [NotMapped]
        public string[] ModifiedProperties { get; set; }


        //Constructors---------------------------------------------------------------------------------------------------------//

        public Person()
        {
            this.ProjectsManager = new HashSet<Project>();
            this.DocumentsAuthor = new HashSet<Document>();
            this.DocumentsReviewer = new HashSet<Document>();
            this.LocationsContact = new HashSet<Location>();
            this.EnteredPrsLogEntrys = new HashSet<PersonLogEntry>();

            this.PersonProjects = new HashSet<Project>();
            this.PersonGroups = new HashSet<PersonGroup>();
            this.ManagedGroups = new HashSet<PersonGroup>();
            this.PersonPrsLogEntrys = new HashSet<PersonLogEntry>();
        }

    }
  

}
