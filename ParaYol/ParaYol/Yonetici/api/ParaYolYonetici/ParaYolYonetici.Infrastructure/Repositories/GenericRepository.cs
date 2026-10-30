using ParaYolYonetici.Core.Enums;
using ParaYolYonetici.Core.Interfaces;
using ParaYolYonetici.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace ParaYolYonetici.Infrastructure.Repositories
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : BaseModel, new()
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly AppDbContext _dbContext;

        protected GenericRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().Where(k => k.SilindiMi == false).OrderBy(k => k.KayitTarihi).ToListAsync();
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> expression)
        {
            return _dbContext.Set<T>().Where(expression);
        }

        public virtual IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            return _dbContext.Set<T>().Where(expression);
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
        }

        public virtual void Delete(T entity)
        {
            entity.SilindiMi = true;
            _dbContext.Set<T>().Update(entity);
        }

        public virtual void Update(T entity, object dto)
        {
            _dbContext.Entry(entity).CurrentValues.SetValues(dto);
        }

        protected KullaniciTip GetUserRole()
        {
            var userRole = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type == ClaimTypes.Role)?.Value;

            if (userRole == null)
            {
                throw new Exception("user role bulunamadı");
            }

            Enum.TryParse(userRole, out KullaniciTip kullaniciTip);
            return kullaniciTip;
        }

        protected bool IsSuperAdmin()
        {
            var userRole = GetUserRole();
            return userRole == KullaniciTip.Admin;
        }

        protected bool IsTedarikciKullanicisi()
        {
            var userRole = GetUserRole();
            return userRole == KullaniciTip.TedarikciKullanicisi;
        }

        protected bool IsWebSiteKullanicisi()
        {
            var userRole = GetUserRole();
            return userRole == KullaniciTip.WebSiteKullanicisi;
        }

        protected Guid GetTedarikciId()
        {
            var id = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type == "tedarikciId")?.Value;
            return Guid.Parse(id);
        }

        protected Guid GetWebSiteId()
        {
            var id = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type == "websiteId")?.Value;
            return Guid.Parse(id);
        }

    }
}
