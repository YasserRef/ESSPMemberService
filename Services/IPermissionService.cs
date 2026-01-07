using ESSPMemberService.Data;

namespace ESSPMemberService.Services
{
    public interface IPermissionService
    {
        bool HasPermission(int userId, string permissionCode);
    }

    public class PermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _context;

        public PermissionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool HasPermission(int userId, string permissionCode)
        {
            try
            {
                return _context.UserPermissions
                                .Any(p => p.F_UserId == userId && p.F_PermissionCode == permissionCode);
            }
            catch
            {
                return false;
            }
        }

    }
}
