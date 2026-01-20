using ESSPMemberService.Data;
using ESSPMemberService.Models.Views;
using Microsoft.EntityFrameworkCore;

namespace ESSPMemberService.Services
{
    public interface IPermissionService
    {
        bool HasPermission(int userId, string permissionCode);  
        List<string> GetUserPermissions(int userId);

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

                var isAdmin = _context.T_USERPERMISSIONS
                .Where(up => up.F_USER_ID == userId && up.F_PAGE_ID == 1).ToList();
                
                if (isAdmin.Count > 0) {
                    return true;
                }

                var hasPermission = _context.T_USERPERMISSIONS
                .Where(up =>
                    up.F_USER_ID == userId &&
                    up.T_PAGE_PERMISSIONS.F_PAGE_NAME == permissionCode).ToList();    

            // var test = _context.V_USER_PAGE_PERMISSIONS
            //.FromSqlRaw("SELECT * FROM ESSP_MOBILE.V_USER_PAGE_PERMISSIONS")
            //.Take(1)
            //.ToList();

            //var hasPermissions = _context.T_USERPERMISSIONS
            //            .Include(up => up.T_PAGE_PERMISSIONS)
            //            .ToList();


            //        // return _context.UserPermissions.Any(p => p.F_UserId == userId && p.F_PAGE_NAME == permissionCode);
            //        var hasPermission = _context.V_USER_PAGE_PERMISSIONS
            //            .Where(p => p.F_USER_ID == userId && p.F_PAGE_NAME == permissionCode).ToList();
                return hasPermission.Count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking permission: {ex.Message}");
                return false;
            }
        }


        public List<string> GetUserPermissions(int userId)
        {
            return _context.V_USER_PAGE_PERMISSIONS
                .Where(up => up.F_USER_ID == userId)
                .Select(up => up.F_PAGE_NAME)
                .ToList();
        }

    }
}
