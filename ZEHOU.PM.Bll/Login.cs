using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ZEHOU.PM.DB.dbLabelInfo;

namespace ZEHOU.PM.Bll
{
    public class Login
    {
        #region 用户
        public User GetUserByLoginName(string loginName)
        {
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    return db.Users.FirstOrDefault(u => u.LoginName == loginName);
                }
            }
            catch (Exception ex) {
                ExceptionTrigger.ProssException("【Login.GetUserByLoginName】按登录名获取用户信息失败", ex);
                return null;
            }
            
        }

        public User GetUserById(string id)
        {
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    return db.Users.Find(id);
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Login.GetUserById】按ID获取用户信息失败", ex);
                return null;
            }
            
        }

        public (User user, List<Role> roles, Department depart, Post post) GetUserAllInfoByUserId(string id)
        {
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    var res = db.Users.Find(id);
                    var roleIds = (res?.RoleID ?? "").Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    var roles = db.Roles.Where(a => roleIds.Contains(a.ID)).ToList();
                    return (res, roles, res?.Department, res?.Post);
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Login.GetUserAllInfoByUserId】获取用户全部信息失败", ex);
                return (null,new List<Role>(),null,null);
            }
            
        }

        public int EditUserPwd(string id, string pwd)
        {
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    var user = db.Users.Find(id);
                    if (user == null) return 0;
                    user.Password = pwd;
                    db.Entry(user).Property(a => a.Password).IsModified = true;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Login.EditUserPwd】修改密码失败", ex);
                return -99;
            }
            
        }

        public List<User> GetUsers(string departId=null,string postId=null,string keyword=null)
        {
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    var where = db.Users.Where(a => true);
                    if (!string.IsNullOrEmpty(departId))
                    {
                        where = where.Where(a => a.Department.ID == departId);
                    }
                    if (!string.IsNullOrEmpty(postId))
                    {
                        where = where.Where(a => a.Department.ID == postId);
                    }
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        where = where.Where(a => a.TrueName.Contains(keyword));
                    }
                    var res = where.OrderBy(a => a.ID).Select(a => new { user = a, depart = a.Department, post = a.Post }).ToList();
                    res.ForEach(a => { a.user.Department = a.depart; a.user.Post = a.post; });

                    return res.Select(a => a.user).ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Login.GetUsers】获取用户列表失败", ex);
                return new List<User>();
            }
            
        }

        public List<User> GetUsers(out int count,int page,int size,string departId = null, string postId = null, string keyword = null)
        {
            count = 0;
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    var where = db.Users.Where(a => true);
                    if (!string.IsNullOrEmpty(departId))
                    {
                        where = where.Where(a => a.Department.ID == departId);
                    }
                    if (!string.IsNullOrEmpty(postId))
                    {
                        where = where.Where(a => a.Department.ID == postId);
                    }
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        where = where.Where(a => a.TrueName.Contains(keyword));
                    }
                    count = where.Count();
                    var res = where.OrderBy(a => a.ID).Select(a => new { user = a, depart = a.Department, post = a.Post }).Skip((page-1)*size).Take(size).ToList();
                    res.ForEach(a => { a.user.Department = a.depart; a.user.Post = a.post; });

                    return res.Select(a => a.user).ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Login.GetUsers】获取用户列表失败", ex);
                return new List<User>();
            }

        }

        public int AddUser(User user)
        {
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    var tmpUsers = db.Users.Where(a => a.ID == user.ID || a.LoginName == user.LoginName).ToList();
                    if (tmpUsers.Any(a => a.ID == user.ID))
                    {
                        return -1;
                    }
                    if (tmpUsers.Any(a => a.LoginName == user.LoginName))
                    {
                        return -2;
                    }
                    db.Users.Add(user);
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Login.AddUser】添加用户失败", ex);
                return -99;
            }
            
        }

        public int EditUser(User user)
        {
            if (string.IsNullOrWhiteSpace(user.ID))
            {
                return -1;
            }
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    db.Users.Attach(user);
                    db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    db.Entry(user).Property(a => a.LoginName).IsModified = false;
                    db.Entry(user).Property(a => a.Password).IsModified = false;


                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Login.EditUser】编辑用户失败", ex);
                return -99;
            }
            
        }

        public int DelUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return -1;
            }
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    var user = db.Users.Find(id);
                    if (user == null)
                    {
                        return -1;
                    }
                    db.Users.Remove(user);
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Login.DelUser】删除用户失败", ex);
                return -99;
            }
            
        }

        #endregion

        #region 角色
        public List<Role> GetRoles()
        {
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    return db.Roles.OrderBy(a => a.OrderID).ThenBy(a => a.ID).ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Login.GetRoles】获取角色列表失败", ex);
                return new List<Role>();
            }
            
        }
        public List<Role> GetRoles(out int count,int page, int size,string keyword=null)
        {
            count = 0;
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    var query = db.Roles.Where(a => true);
                    if (!string.IsNullOrEmpty(keyword)) {
                        query = query.Where(a=>a.Name.Contains(keyword));
                    }
                    count = query.Count();
                    return query.OrderBy(a => a.OrderID).ThenBy(a => a.ID).Skip((page-1)*size).Take(size).ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Login.GetRoles】获取角色列表失败", ex);
                return new List<Role>();
            }

        }

        public int EditRoleFunc(Role role)
        {
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    if (role == null) return 0;
                    db.Roles.Attach(role);
                    db.Entry(role).Property(a => a.FunctionID).IsModified = true;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Login.EditRoleFunc】修改角色权限失败", ex);
                return -99;
            }
            
        }

        public Role GetRoleById(string id)
        {
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    return db.Roles.Find(id);
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Login.GetRoleById】按ID获取角色失败", ex);
                return null;
            }
            
        }

        public int AddRole(Role role)
        {
            var tmprole = GetRoleById(role.ID);
            if (tmprole != null)
            {
                return -1;
            }
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    db.Roles.Add(role);
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Login.AddRole】添加角色失败", ex);
                return -99;
            }
            
        }

        public int EditRole(Role role)
        {
            if (string.IsNullOrWhiteSpace(role.ID))
            {
                return -1;
            }
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    db.Roles.Attach(role);
                    db.Entry(role).State = System.Data.Entity.EntityState.Modified;
                    db.Entry(role).Property(a => a.FunctionID).IsModified = false;

                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Login.EditRole】修改角色失败", ex);
                return -99;
            }
            
        }

        public int DelRole(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return -1;
            }
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    var role = new Role { ID = id };
                    db.Roles.Attach(role);
                    db.Entry(role).State = System.Data.Entity.EntityState.Deleted;

                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Login.DelRole】删除角色失败", ex);
                return -99;
            }
            
        }
        #endregion

        #region 部门
        public List<Department> GetDepartments()
        {
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    return db.Departments.OrderBy(a => a.ID).ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Login.GetDepartments】获取部门列表失败", ex);
                return new List<Department>();
            }
            
        }

        public Department GetDepartmentById(string id)
        {
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    return db.Departments.Find(id);
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Login.GetDepartmentById】按ID获取部门失败", ex);
                return null;
            }
            
        }

        public int AddDepartment(Department depart)
        {
            var tmpDepartment = GetDepartmentById(depart.ID);
            if (tmpDepartment != null)
            {
                return -1;
            }
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    db.Departments.Add(depart);
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Login.AddDepartment】添加部门失败", ex);
                return -99;
            }
            
        }

        public int EditDepartment(Department depart)
        {
            if (string.IsNullOrWhiteSpace(depart.ID))
            {
                return -1;
            }
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    db.Departments.Attach(depart);
                    db.Entry(depart).State = System.Data.Entity.EntityState.Modified;

                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Login.EditDepartment】编辑部门失败", ex);
                return -99;
            }
            
        }

        public int DelDepartment(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return -1;
            }
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    var depart = new Department { ID = id };
                    db.Departments.Attach(depart);
                    db.Entry(depart).State = System.Data.Entity.EntityState.Deleted;

                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Login.DelDepartment】删除部门失败", ex);
                return -99;
            }
            
        }
        #endregion

        #region 职务
        public List<Post> GetPosts()
        {
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    return db.Posts.OrderBy(a => a.OrderID).ThenBy(a => a.ID).ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Login.GetPosts】获取职务列表失败", ex);
                return new List<Post>();
            }
            
        }

        public Post GetPostById(string id)
        {
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    return db.Posts.Find(id);
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Login.GetPostById】按ID获取职务失败", ex);
                return null;
            }
            
        }

        public int AddPost(Post post)
        {
            var tmpPost = GetPostById(post.ID);
            if (tmpPost != null)
            {
                return -1;
            }
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    db.Posts.Add(post);
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Login.AddPost】添加职务失败", ex);
                return -99;
            }
            
        }

        public int EditPost(Post post)
        {
            if (string.IsNullOrWhiteSpace(post.ID))
            {
                return -1;
            }
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    db.Posts.Attach(post);
                    db.Entry(post).State = System.Data.Entity.EntityState.Modified;

                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Login.EditPost】编辑职务失败", ex);
                return -99;
            }
            
        }

        public int DelPost(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return -1;
            }
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    var post = new Post { ID = id };
                    db.Posts.Attach(post);
                    db.Entry(post).State = System.Data.Entity.EntityState.Deleted;

                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Login.DelPost】删除职务失败", ex);
                return -99;
            }
            
        }
        #endregion

    }
}
