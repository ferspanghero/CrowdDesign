﻿using CrowdDesign.Core.Entities;
namespace CrowdDesign.UI.Web.Models.Project
{
    public class EditSketchViewModel
    {
        #region Constructors
        public EditSketchViewModel()
            : this(null)
        { }

        public EditSketchViewModel(Sketch sketch)
        {
            if (sketch != null)
            {
                SketchId = sketch.Id;
                Data = sketch.Data;
                ImageUri = sketch.ImageUri;
                Position = sketch.Position;

                if (sketch.Dimension != null)
                {
                    DimensionId = sketch.Dimension.Id;

                    if (sketch.Dimension.Project != null)
                        ProjectId = sketch.Dimension.Project.Id;
                }

                if (sketch.User != null)
                    UserId = sketch.User.Id;
            }
        }
        #endregion

        #region Properties
        public int? ProjectId { get; set; }
        public int? DimensionId { get; set; }
        public int? SketchId { get; set; }
        public int? UserId { get; set; }
        public string Data { get; set; }
        public string ImageUri { get; set; }
        public int Position { get; set; }
        #endregion

        #region Methods
        public Sketch ToDomainModel()
        {
            Sketch sketch = new Sketch
            {
                Id = SketchId ?? -1,
                Data = Data,
                ImageUri = ImageUri,
                Position = Position,
                User = new User {Id = UserId ?? -1},
                Dimension = new Dimension
                {
                    Id = DimensionId ?? -1,
                    Project = new Core.Entities.Project {Id = ProjectId ?? -1}
                }
            };

            return
                sketch;
        }
        #endregion
    }
}