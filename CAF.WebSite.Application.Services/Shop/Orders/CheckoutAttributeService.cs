using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.WebSite.Domain.Seedwork.Shop.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
 

namespace CAF.WebSite.Application.Services.Orders
{
    /// <summary>
    /// Checkout attribute service
    /// </summary>
    public partial class CheckoutAttributeService : ICheckoutAttributeService
    {
        #region Constants

        private const string CHECKOUTATTRIBUTES_ALL_KEY = "WebSite.checkoutattribute.all-{0}";
        private const string CHECKOUTATTRIBUTEVALUES_ALL_KEY = "WebSite.checkoutattributevalue.all-{0}";
        private const string CHECKOUTATTRIBUTES_PATTERN_KEY = "WebSite.checkoutattribute.";
        private const string CHECKOUTATTRIBUTEVALUES_PATTERN_KEY = "WebSite.checkoutattributevalue.";
        private const string CHECKOUTATTRIBUTES_BY_ID_KEY = "WebSite.checkoutattribute.id-{0}";
        private const string CHECKOUTATTRIBUTEVALUES_BY_ID_KEY = "WebSite.checkoutattributevalue.id-{0}";

        #endregion
        
        #region Fields

        private readonly IRepository<CheckoutAttribute> _checkoutAttributeRepository;
        private readonly IRepository<CheckoutAttributeValue> _checkoutAttributeValueRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;
        
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="checkoutAttributeRepository">Checkout attribute repository</param>
        /// <param name="checkoutAttributeValueRepository">Checkout attribute value repository</param>
        /// <param name="eventPublisher">Event published</param>
        public CheckoutAttributeService(ICacheManager cacheManager,
            IRepository<CheckoutAttribute> checkoutAttributeRepository,
            IRepository<CheckoutAttributeValue> checkoutAttributeValueRepository,
            IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _checkoutAttributeRepository = checkoutAttributeRepository;
            _checkoutAttributeValueRepository = checkoutAttributeValueRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        #region Checkout attributes

        /// <summary>
        /// Deletes a checkout attribute
        /// </summary>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        public virtual void DeleteCheckoutAttribute(CheckoutAttribute checkoutAttribute)
        {
            if (checkoutAttribute == null)
                throw new ArgumentNullException("checkoutAttribute");

            _checkoutAttributeRepository.Delete(checkoutAttribute);

            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(checkoutAttribute);
        }

        /// <summary>
        /// Gets all checkout attributes
        /// </summary>
        /// <returns>Checkout attribute collection</returns>
        public virtual IList<CheckoutAttribute> GetAllCheckoutAttributes(bool showHidden = false)
        {
			string key = CHECKOUTATTRIBUTES_ALL_KEY.FormatInvariant(showHidden);

            return _cacheManager.Get(key, () =>
            {
				var query = _checkoutAttributeRepository.Table;

				if (!showHidden)
					query = query.Where(x => x.IsActive);

				query = query.OrderBy(x => x.DisplayOrder);

                var checkoutAttributes = query.ToList();
                return checkoutAttributes;
            });
        }

        /// <summary>
        /// Gets a checkout attribute 
        /// </summary>
        /// <param name="checkoutAttributeId">Checkout attribute identifier</param>
        /// <returns>Checkout attribute</returns>
        public virtual CheckoutAttribute GetCheckoutAttributeById(int checkoutAttributeId)
        {
            if (checkoutAttributeId == 0)
                return null;

            string key = string.Format(CHECKOUTATTRIBUTES_BY_ID_KEY, checkoutAttributeId);
            return _cacheManager.Get(key, () => 
            { 
                return _checkoutAttributeRepository.GetById(checkoutAttributeId); 
            });
        }

        /// <summary>
        /// Inserts a checkout attribute
        /// </summary>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        public virtual void InsertCheckoutAttribute(CheckoutAttribute checkoutAttribute)
        {
            if (checkoutAttribute == null)
                throw new ArgumentNullException("checkoutAttribute");

            _checkoutAttributeRepository.Insert(checkoutAttribute);

            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(checkoutAttribute);
        }

        /// <summary>
        /// Updates the checkout attribute
        /// </summary>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        public virtual void UpdateCheckoutAttribute(CheckoutAttribute checkoutAttribute)
        {
            if (checkoutAttribute == null)
                throw new ArgumentNullException("checkoutAttribute");

            _checkoutAttributeRepository.Update(checkoutAttribute);

            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(checkoutAttribute);
        }

        #endregion

        #region Checkout variant attribute values

        /// <summary>
        /// Deletes a checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValue">Checkout attribute value</param>
        public virtual void DeleteCheckoutAttributeValue(CheckoutAttributeValue checkoutAttributeValue)
        {
            if (checkoutAttributeValue == null)
                throw new ArgumentNullException("checkoutAttributeValue");

            _checkoutAttributeValueRepository.Delete(checkoutAttributeValue);

            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(checkoutAttributeValue);
        }

        /// <summary>
        /// Gets checkout attribute values by checkout attribute identifier
        /// </summary>
        /// <param name="checkoutAttributeId">The checkout attribute identifier</param>
        /// <returns>Checkout attribute value collection</returns>
        public virtual IList<CheckoutAttributeValue> GetCheckoutAttributeValues(int checkoutAttributeId)
        {
            string key = string.Format(CHECKOUTATTRIBUTEVALUES_ALL_KEY, checkoutAttributeId);
            return _cacheManager.Get(key, () =>
            {
                var query = from cav in _checkoutAttributeValueRepository.Table
                            orderby cav.DisplayOrder
                            where cav.CheckoutAttributeId == checkoutAttributeId
                            select cav;
                var checkoutAttributeValues = query.ToList();
                return checkoutAttributeValues;
            });
        }
        
        /// <summary>
        /// Gets a checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValueId">Checkout attribute value identifier</param>
        /// <returns>Checkout attribute value</returns>
        public virtual CheckoutAttributeValue GetCheckoutAttributeValueById(int checkoutAttributeValueId)
        {
            if (checkoutAttributeValueId == 0)
                return null;

            string key = string.Format(CHECKOUTATTRIBUTEVALUES_BY_ID_KEY, checkoutAttributeValueId);
            return _cacheManager.Get(key, () => 
            { 
                return _checkoutAttributeValueRepository.GetById(checkoutAttributeValueId); 
            });
        }

        /// <summary>
        /// Inserts a checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValue">Checkout attribute value</param>
        public virtual void InsertCheckoutAttributeValue(CheckoutAttributeValue checkoutAttributeValue)
        {
            if (checkoutAttributeValue == null)
                throw new ArgumentNullException("checkoutAttributeValue");

            _checkoutAttributeValueRepository.Insert(checkoutAttributeValue);

            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(checkoutAttributeValue);
        }

        /// <summary>
        /// Updates the checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValue">Checkout attribute value</param>
        public virtual void UpdateCheckoutAttributeValue(CheckoutAttributeValue checkoutAttributeValue)
        {
            if (checkoutAttributeValue == null)
                throw new ArgumentNullException("checkoutAttributeValue");

            _checkoutAttributeValueRepository.Update(checkoutAttributeValue);

            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(checkoutAttributeValue);
        }
        
        #endregion

        #endregion
    }
}
