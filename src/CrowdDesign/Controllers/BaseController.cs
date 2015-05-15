using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrowdDesign.Core.Interfaces.Entities;
using CrowdDesign.Core.Interfaces.Repositories;
using CrowdDesign.Utils.Extensions;

namespace CrowdDesign.UI.Web.Controllers
{
    public class BaseController<TRepository, TEntity, TKey> : Controller
        where TRepository : class, IBaseRepository<TEntity, TKey>
        where TEntity : class, IDomainEntity<TKey>
    {
        #region Constructor
        public BaseController(TRepository repository)
        {
            repository.TryThrowArgumentNullException("repository");

            Repository = repository;
        } 
        #endregion

        #region Fields
        protected readonly TRepository Repository;
        #endregion

        #region Methods
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Repository.Dispose();

            base.Dispose(disposing);
        }
        #endregion
    }
}
