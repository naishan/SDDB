﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using SDDB.Domain.Abstract;
using SDDB.Domain.Infrastructure;

namespace SDDB.Domain.Entities
{
    [Table("AssemblyDbs")]
    public class AssemblyDb : IDbEntity
    {
        //Entity Framework Properties------------------------------------------------------------------------------------------//

        [Key]
        [StringLength(40)]
        public string Id { get; set; }

        [Required(ErrorMessage = "Assy. Name field is required")]
        [DBIsUnique] [Index(IsUnique = true)]
        [StringLength(255)]
        public string AssyName { get; set; }

        [DBIsUnique] [Index(IsUnique = true)]
        [StringLength(255)]
        public string AssyAltName { get; set; }

        [StringLength(255)]
        public string AssyAltName2 { get; set; }

        [Required( ErrorMessage = "Assy. Type field is required")]
        [StringLength(40)]
        [ForeignKey("AssemblyType")]
        public string AssemblyType_Id { get; set; }

        [Required(ErrorMessage = "Assy. Status field is required")]
        [StringLength(40)]
        [ForeignKey("AssemblyStatus")]
        public string AssemblyStatus_Id { get; set; }

        [StringLength(40)]
        [ForeignKey("AssemblyModel")]
        public string AssemblyModel_Id { get; set; }

        [Required(ErrorMessage = "Project field is required")]
        [StringLength(40)]
        [ForeignKey("AssignedToProject")]
        public string AssignedToProject_Id { get; set; }

        [Required(ErrorMessage = "Location field is required")]
        [StringLength(40)]
        [ForeignKey("AssignedToLocation")]
        public string AssignedToLocation_Id { get; set; }

        [Required(ErrorMessage = "Assy. Global Coord's are required")]
        public decimal AssyGlobalX { get; set; }

        [Required(ErrorMessage = "Assy. Global Coord's are required")]
        public decimal AssyGlobalY { get; set; }

        [Required(ErrorMessage = "Assy. Global Coord's are required")]
        public decimal AssyGlobalZ { get; set; }

        public decimal? AssyLocalXDesign { get; set; }

        public decimal? AssyLocalYDesign { get; set; }

        public decimal? AssyLocalZDesign { get; set; }

        public decimal? AssyLocalXAsBuilt { get; set; }

        public decimal? AssyLocalYAsBuilt { get; set; }

        public decimal? AssyLocalZAsBuilt { get; set; }

        public decimal? AssyStationing { get; set; }

        public decimal? AssyLength { get; set; }

        public decimal? AssyReadingIntervalSecs { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool IsReference_bl { get; set; }
               
        [Column(TypeName = "text")] [StringLength(65535)]
        public string TechnicalDetails { get; set; }

        [Column(TypeName = "text")] [StringLength(65535)]
        public string PowerSupplyDetails { get; set; }

        [Column(TypeName = "text")] [StringLength(65535)]
        public string HSEDetails { get; set; }

        [Column(TypeName = "text")] [StringLength(65535)]
        public string Comments { get; set; }

        [Required]
        [DefaultValue(true)]
        public bool IsActive_bl { get; set; }

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
        
        //one to many
        public virtual AssemblyType AssemblyType { get; set; }
        public virtual AssemblyStatus AssemblyStatus { get; set; }
        public virtual AssemblyModel AssemblyModel { get; set; }
        public virtual Project AssignedToProject { get; set; }
        public virtual Location AssignedToLocation { get; set; }
        
        public virtual AssemblyExt AssemblyExt { get; set; }

        [InverseProperty("AssignedToAssemblyDb")]
        public virtual ICollection<Component> AssemblyComponents { get; set; }

        [InverseProperty("AssemblyDb")]
        public virtual ICollection<AssemblyLogEntry> AssemblyLogEntrys { get; set; }

        //many to many

        [InverseProperty("PrsLogEntryAssemblyDbs")]
        public virtual ICollection<PersonLogEntry> AssemblyDbPrsLogEntrys { get; set; }


        //Non-persistent Properties--------------------------------------------------------------------------------------------//

        [NotMapped]
        public string[] ModifiedProperties { get; set; }


        //Constructors---------------------------------------------------------------------------------------------------------//

        public AssemblyDb()
        {
            this.AssemblyComponents = new HashSet<Component>();
            this.AssemblyLogEntrys = new HashSet<AssemblyLogEntry>();
            this.AssemblyDbPrsLogEntrys = new HashSet<PersonLogEntry>();
        }

        

    }
  



}
