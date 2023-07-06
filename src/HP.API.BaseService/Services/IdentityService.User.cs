using System;
using System.IO;
using System.Linq;
using System.Net;
using HP.Core.Configs;
using HP.Core.Mapping;
using HP.Core.Security;
using HP.Core.Security.Permissions;
using HP.Data.Orm;
using HP.Utility.Data;
using HP.Utility.Extensions;
using HP.Web.Mvc.Utility;
using HPC.BaseService.Dtos;
using HPC.BaseService.Models;
using HPC.BaseService.Resources;

namespace HPC.BaseService.Services
{
    public partial class IdentityService
    {
        public IMapper Mapper { set; get; }

        public IQuery<User> Users
        {
            get { return UserRepository.Query(); }
        }

        public IQuery<User> WholeUsers
        {
            get { return UserRepository.WholeQuery(); }
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="entityDto"></param>
        /// <returns></returns>
        public DataResult CreateUser(UserInputDto entityDto)
        {

            // 验证创建信息
            DataResult result = ValidateUser(entityDto);
            if (!result.Success) return result;

            // 转换实体
            var entity = Mapper.MapTo<User>(entityDto);

            // 生成用户编码
            //entity.Code = SequenceContract.Create(entity.GetType());

            // 验证用户编码
            if (entity.Code.IsNullOrEmpty())
            {
                return DataProcess.Failure("用户编码不能为空！");
            }

            // 验证密码编码
            if (entity.Password.IsNullOrEmpty())
            {
                return DataProcess.Failure("用户密码不能为空！");
            }
            entity.Password = entity.Password.ToUpper();

            //验证用户编码是否存在
            if (Users.Any(a => a.Code == entity.Code))
            {
                return DataProcess.Failure("员工编码({0})已经存在！".FormatWith(entity.Code));
            }
            //验证用户编码是否存在
            //if (Users.Any(a => a.RFIDCode == entity.RFIDCode))
            //{
            //    return DataProcess.Failure("员工卡号({0})已经存在！".FormatWith(entity.Code));
            //}
            UserRepository.UnitOfWork.TransactionEnabled = true;    
            //插入用户
            if (!UserRepository.Insert(entity))
            {
                return DataProcess.Failure("用户({0})创建失败！".FormatWith(entity.Id));
            }
            //用户角色
            var rolesMap = new UserRolesMapInputDto
            {
                UserCode = entity.Code,
                RoleCodes = entityDto.Roles
            };
            if (! SetUserRoles(rolesMap).Success)
            {
                return DataProcess.Failure("用户({0})授权失败！".FormatWith(entity.Id));
            }
            
            UserRepository.UnitOfWork.Commit();
            return DataProcess.Success("用户({0})创建成功！".FormatWith(entity.Id));
        }

        /// <summary>
        /// 编辑用户个人中心信息
        /// </summary>
        /// <param name="entityDto"></param>
        /// <returns></returns>
        public DataResult EditUserCenter(UserInputDto entityDto)
        {

            var oriEntity = Users.Where(a => a.Id == entityDto.Id).Select(a => new { a.Code, a.IsSystem }).FirstOrDefault();
            oriEntity.CheckNotNull("oriEntity");


            //编辑用户
            if (UserRepository.Update(a => new User()
            {
                Code = entityDto.Code,
                Name = entityDto.Name,
                Sex = entityDto.Sex,
                Mobilephone = entityDto.Mobilephone,
                Password = entityDto.Password,
                WeXin = entityDto.WeXin,
                RFIDCode = entityDto.RFIDCode,
                PictureUrl = entityDto.PictureUrl,
                Remark = entityDto.Remark,
                Enabled = entityDto.Enabled,
                Header = entityDto.Header,
                FileID = entityDto.FileId
            }, a => a.Id == entityDto.Id) == 0)
            {
                return DataProcess.Failure("用户({0})编辑失败！".FormatWith(oriEntity.Code));
            }
       
            return DataProcess.Success("用户({0})编辑成功！".FormatWith(oriEntity.Code));
        }

        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <param name="entityDto"></param>
        /// <returns></returns>
        public DataResult EditUser(UserInputDto entityDto)
        {

            var oriEntity = Users.Where(a => a.Id == entityDto.Id).Select(a => new {a.Code, a.IsSystem}).FirstOrDefault();
            oriEntity.CheckNotNull("oriEntity");

            //如果是系统用户
            if (oriEntity.IsSystem)
            {
                return DataProcess.Failure("系统帐号无法编辑！");
            }

            //编辑用户
            if (UserRepository.Update(a => new User()
            {
                Code = entityDto.Code,
                Name = entityDto.Name,
                Sex = entityDto.Sex,
                Mobilephone = entityDto.Mobilephone,
                Password = entityDto.Password,
                WeXin = entityDto.WeXin,
                RFIDCode=entityDto.RFIDCode,
                PictureUrl= entityDto.PictureUrl,
                Remark = entityDto.Remark,
                Enabled = entityDto.Enabled,
                Header = entityDto.Header,
                FileID=entityDto.FileId
            }, a => a.Id == entityDto.Id) == 0)
            {
                return DataProcess.Failure("用户({0})编辑失败！".FormatWith(oriEntity.Code));
            }
            var rolesMap = new UserRolesMapInputDto
            {
                UserCode = entityDto.Code,
                RoleCodes = entityDto.Roles
            };
            if (!SetUserRoles(rolesMap).Success)
            {
                return DataProcess.Failure("用户({0})授权失败！".FormatWith(entityDto.Id));
            }

            return DataProcess.Success("用户({0})编辑成功！".FormatWith(oriEntity.Code));
        }

        /// <summary>
        /// 编辑用户联系方式
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult EditUserContract(User entity)
        {
            entity.CheckNotNull("entity");
            entity.Id.CheckGreaterThan("Id", 0);

            var oriEntity = Users.FirstOrDefault(a => a.Id == entity.Id);
            oriEntity.CheckNotNull("oriEntity");

            return UserRepository.Update(a => new User
            {
                Telephone = entity.Telephone,
                Mobilephone = entity.Mobilephone,
                Email = entity.Email,
                WeXin = entity.WeXin,
            }, a => a.Id == entity.Id) > 0
                ? DataProcess.Success("用户({0})联系方式编辑成功！".FormatWith(oriEntity.Code))
                : DataProcess.Failure("用户({0})联系方式编辑失败！".FormatWith(oriEntity.Code));
        }

        /// <summary>
        /// 编辑用户头像
        /// </summary>
        /// <returns></returns>
        public DataResult EditUserHeader(User entity)
        {
            entity.CheckNotNull("entity");

            var oriEntity = Users.FirstOrDefault(a => a.Id == entity.Id);
            oriEntity.CheckNotNull("oriEntity");

            return UserRepository.Update(a => new User
            {
                Header = entity.Header
            }, a => a.Id == entity.Id) > 0
                ? DataProcess.Success("用户({0})头像编辑成功！".FormatWith(oriEntity.Code), entity.Header)
                : DataProcess.Failure("用户({0})头像编辑失败！".FormatWith(oriEntity.Code));
        }


        /// <summary>
        /// 移除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataResult RemoveUser(int id)
        {
            id.CheckGreaterThan("id", 0);

            var oriEntity = Users.Where(a => a.Id == id).Select(a => new {a.IsSystem, a.Code}).First();

            if (oriEntity.IsSystem)
            {
                return DataProcess.Failure("用户({0})是系统用户，无法移除！".FormatWith(oriEntity.Code));
            }

            if (UserRepository.LogicDelete(id) == 0)
            {
                return DataProcess.Failure("用户({0})移除失败".FormatWith(oriEntity.Code));
            }

            return DataProcess.Success("用户({0})移除成功！".FormatWith(oriEntity.Code));
        }

        /// <summary>
        /// 还原用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataResult RestoryUser(int id)
        {
            id.CheckGreaterThan("id", 0);

            var oriEntity = Users.Where(a => a.Id == id).Select(a => new {a.Code}).FirstOrDefault();
            oriEntity.CheckNotNull("oriEntity");

            return UserRepository.Restory(id) > 0
                ? DataProcess.Success("用户({0})还原成功！".FormatWith(oriEntity.Code))
                : DataProcess.Failure("用户({0})还原失败！".FormatWith(oriEntity.Code));
        }

        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        public DataResult ResetUserPassword(UserPasswordInputDto inputDto)
        {
            inputDto.CheckNotNull("inputDto");
            inputDto.Code.CheckNotNullOrEmpty("Code");

            if (UserRepository.Update(a => new User
            {
                Password = inputDto.Password.ToUpper()
            }, a => a.Code == inputDto.Code) == 0)
            {
                return DataProcess.Failure("用户({0})密码重置失败！");
            }

            return DataProcess.Success("用户({0})密码重置成功！".FormatWith(inputDto.Code));
        }

        /// <summary>
        /// 设置用户密码
        /// </summary>
        public DataResult SetUserPassword(UserPasswordInputDto inputDto)
        {
            inputDto.CheckNotNull("inputDto");
            inputDto.Code.CheckNotNullOrEmpty("Code");

            if (inputDto.OriginalPassword.IsNullOrEmpty())
            {
                return DataProcess.Failure("用户({0})原始密码不能为空！".FormatWith(inputDto.Code));
            }

            //验证原始密码
            inputDto.OriginalPassword = inputDto.OriginalPassword.ToUpper();
            if (!Users.Any(a => a.Code == inputDto.Code && a.Password == inputDto.OriginalPassword))
            {
                return DataProcess.Failure("用户({0})原始密码验证失败！".FormatWith(inputDto.Code));
            }

            if (UserRepository.Update(a => new User
            {
                Password = inputDto.Password.ToUpper()
            }, a => a.Code == inputDto.Code) == 0)
            {
                return DataProcess.Failure("用户({0})密码设置失败！");
            }

            return DataProcess.Success("用户({0})密码设置成功！".FormatWith(inputDto.Code));
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        public DataResult Login(LoginInfo inputDto)
        {
            //系统授权
            //if (!Register.Validate.CheckWeb(false))
            //{
            //    return DataProcess.Failure("该版本未授权或已过期，请联系管理员授权！");
            //};
            #region 数据验证

            //基础数据验证
            if (inputDto.Code.IsNullOrEmpty())
            {
                return DataProcess.Failure("用户编码不能为空！");
            }
            if (inputDto.Password.IsNullOrEmpty())
            {
                return DataProcess.Failure("用户密码不能为空！");
            }

            //验证用户是否存在
            var entity = Users.Where(a => a.Code == inputDto.Code && a.Password == inputDto.Password).Select(a => new { a.Enabled, a.IsSystem, a.Code, a.Name, a.Header }).FirstOrDefault();
            if (entity == null)
            {
                return DataProcess.Failure("用户({0})不存在或密码错误！".FormatWith(inputDto.Code));
            }

            //验证用户是否启用
            if (!entity.Enabled)
            {
                return DataProcess.Failure("用户({0})已被禁用或尚未启用！".FormatWith(entity.Code));
            }

            #endregion

            #region 生成身份票据

            UserData userData = new UserData
            {
                Code = entity.Code,
                Name = entity.Name,
                Header = entity.Header,//,
                IsSystem = entity.IsSystem,
                RoleIds = RoleUsersMaps.InnerJoin(Roles, (map, role) => map.RoleCode == role.Code)
                    .Select((map, role) => new
                    {
                        map,
                        role
                    })
                    .Where(a => a.map.UserCode == entity.Code && a.role.Enabled)
                    .Select(a => a.map.RoleCode)
                    .ToList()
                    .ToArray()
            };

            IdentityTicket identity = new IdentityTicket
            {
                SessionId = inputDto.SessionId,
                Expires = HPConfig.Instance.IdentityExpires,
                LastLoginTime = DateTime.Now,
                Ip = inputDto.ClientIp,
                UserData = userData
            };

            #endregion

            #region 注册身份票据

            DataResult result = IdentityManager.Register(identity);
            if (!result.Success) return result;

            #endregion

            #region 初始化用户权限

            //如果非系统用户，则需要初始化授权数据
            //if (!entity.IsSystem)
            //{
            //    Authorization.Initialize();
            //}

            #endregion

            return DataProcess.Success("用户({0})登录成功！".FormatWith(entity.Code), identity.Token);
        }


        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public DataResult GetUserInfo(string token)
        {
            string tokenMaster = token; 
            if (tokenMaster.IsNullOrEmpty())
            {
                tokenMaster = CookieHelper.Get("token");
                if (tokenMaster.IsNullOrEmpty())
                {
                    return DataProcess.Failure("获取用户信息失败");
                }
            }
            IdentityTicket identity = SecurityResolver.Resolve(tokenMaster);
            return DataProcess.Success("获取用户信息成功！", identity.UserData);
        }


        /// <summary>
        /// 头像
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public string GetHeader(string header)
        {
            string result = CatalogResource.Catalog_Header + "/Avatar.gif";
            if (!header.IsNullOrEmpty())
            {
                result = CatalogResource.Catalog_Header + "/" + header;
            }
            return result;
        }

        /// <summary>
        /// 验证数据合法性
        /// </summary>
        /// <param name="entityDto"></param>
        /// <returns></returns>
        private DataResult ValidateUser(UserInputDto entityDto)
        {
            if (entityDto.Roles.IsNullOrEmpty())
            {
                return DataProcess.Failure("员工角色不能为空！");
            }
            //if (entityDto.Mobilephone.IsNullOrEmpty())
            //{
            //    return DataProcess.Failure("员工手机号不能为空！");
            //}
            if (entityDto.Name.IsNullOrEmpty())
            {
                return DataProcess.Failure("员工名称不能为空！");
            }
            return DataProcess.Success();
        }

        /// <summary>
        /// 客户端登录
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        public DataResult UserLogin(LoginInfo inputDto)
        {
            //系统授权
            //if (!Register.Validate.Check(false))
            //{
            //    return DataProcess.Failure("该版本未授权或已过期，请联系管理员授权！");
            //};
            #region 数据验证

            var entity = new User();
            if (!inputDto.VerifyCode.IsNullOrEmpty())
            {
                entity = Users.FirstOrDefault(a => a.RFIDCode == inputDto.VerifyCode);
                if (entity == null)
                {
                    return DataProcess.Failure("员工卡号({0})不存在！".FormatWith(inputDto.VerifyCode));
                }
            }
            else
            {
                //基础数据验证
                if (inputDto.Code.IsNullOrEmpty())
                {
                    return DataProcess.Failure("用户编码不能为空！");
                }
                if (inputDto.Password.IsNullOrEmpty())
                {
                    return DataProcess.Failure("用户密码不能为空！");
                }

                //验证用户是否存在
                entity = Users.FirstOrDefault(a => a.Code == inputDto.Code && a.Password == inputDto.Password);
                if (entity == null)
                {
                    return DataProcess.Failure("用户({0})不存在或密码错误！".FormatWith(inputDto.Code));
                }
            }

            //验证用户是否启用
            if (!entity.Enabled)
            {
                return DataProcess.Failure("用户({0})已被禁用或尚未启用！".FormatWith(entity.Code));
            }

            #endregion

            #region 生成身份票据

            UserData userData = new UserData
            {
                Code = entity.Code,
                Name = entity.Name,
                Header = entity.PictureUrl,//GetHeader(),//
                IsSystem = entity.IsSystem,
                DeptId = entity.DeptId,
                Leader = entity.Leader,
                RoleIds = RoleUsersMaps.InnerJoin(Roles, (map, role) => map.RoleCode == role.Code)
                    .Select((map, role) => new
                    {
                        map,
                        role
                    })
                    .Where(a => a.map.UserCode == entity.Code && a.role.Enabled)
                    .Select(a => a.map.RoleCode)
                    .ToList()
                    .ToArray()
            };

            IdentityTicket identity = new IdentityTicket
            {
                SessionId = inputDto.SessionId,
                Expires = HPConfig.Instance.IdentityExpires,
                LastLoginTime = DateTime.Now,
                Ip = inputDto.ClientIp,
                UserData = userData
            };

            #endregion

            #region 注册身份票据

            DataResult result = IdentityManager.Register(identity);
            if (!result.Success) return result;

            #endregion

            #region 初始化用户权限

            //如果非系统用户，则需要初始化授权数据
            //if (!entity.IsSystem)
            //{
            //    Authorization.Initialize();
            //}

            #endregion

            return DataProcess.Success("用户({0})登录成功！".FormatWith(entity.Code), identity);
        }

        /// <summary>
        /// 获取微信OpenId
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private string GetOpenId(string code, string appId,string appSecret)
        {
            string serviceAddress = "https://api.weixin.qq.com/sns/jscode2session?appid=" + appId + "&secret=" + appSecret + "&js_code=" + code + "&grant_type=authorization_code";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serviceAddress);
            request.Method = "GET";
            request.ContentType = "text/html;charset=utf-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, System.Text.Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            string openid = null;
            string key = "\"openid\":\"";
            int startIndex = retString.IndexOf(key);
            if (startIndex != -1)
            {
                int endIndex = retString.IndexOf("}", startIndex);
                openid = retString.Substring(startIndex + key.Length, endIndex - startIndex - key.Length - 1);
            }
            else
            {
                return null;
            }
            return openid;
        }
    }
}
