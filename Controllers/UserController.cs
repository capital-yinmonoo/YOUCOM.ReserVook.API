using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.ReserVook.API.Tools;

namespace YOUCOM.ReserVook.API.Controllers
{
    /// <summary>
    /// ユーザ管理コントローラー
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private IUserService _userService;
        private ICodeNameService _dictionaryDataService;

        public UserController(ILogger<UserController> logger, 
            IUserService userService,
            ICodeNameService dictionaryDataService)
        {
            _logger = logger;

            _userService = userService;

            _dictionaryDataService = dictionaryDataService;
        }

        #region マスタ関係

        // 情報取得(画面表示用)
        [HttpGet("getList")]
        public async Task<List<MstUserInfo>> GetList()
        {
            var model = await _userService.GetList();
            return model;
        }

        // 情報取得(編集・削除用)
        [HttpPost("getByIdUser")]
        public async Task<MstUserInfo> GetById(MstUserInfo userInfo)
        {
            return await _userService.GetById(userInfo);
        }

        // 追加
        [HttpPost("addUser")]
        public async Task<int> AddUser(MstUserInfo userInfo)
        {
            int addFlg = 0;
            try
            {
                var result = await _userService.AddUser(userInfo);
                if (result == 0)
                {
                    return addFlg;
                }
                else
                {
                    addFlg = 1;
                    return addFlg;
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                addFlg = -1;
                return addFlg;
            }
        }


        // 更新
        [HttpPut("updateUser")]
        public async Task<CommonEnum.DBUpdateResult> UpdateUser(MstUserInfo userInfo)
        {
            try
            {
                bool addFlag = false;
                int result = await _userService.UpdateUser(userInfo, addFlag);
                if (result == 1)
                {
                    return CommonEnum.DBUpdateResult.Success;
                }
                else if (result == -1)
                {
                    return CommonEnum.DBUpdateResult.VersionError;
                }
                else if (result == -3)
                {
                    return CommonEnum.DBUpdateResult.OverlapError;
                }
                else
                {
                    return CommonEnum.DBUpdateResult.Error;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return CommonEnum.DBUpdateResult.Error;
            }
        }

        // 削除
        [HttpPost("delUser")]
        public async Task<CommonEnum.DBUpdateResult> DelUser(MstUserInfo userInfo)
        {
            try
            {
                int ret = await _userService.DelUser(userInfo);
                if (ret == 1)
                {
                    return CommonEnum.DBUpdateResult.Success;
                }
                else if (ret == -1)
                {
                    return CommonEnum.DBUpdateResult.VersionError;
                }
                else
                {
                    return CommonEnum.DBUpdateResult.Error;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return CommonEnum.DBUpdateResult.Error;
            }
        }

        #endregion

        #region ログイン関係
        /// <summary>
        /// ログイン処理
        /// </summary>
        /// <param name="user">ユーザ情報(UserEmail,Passwordを渡す)</param>
        /// <returns></returns>
        [HttpPost("emailAndPwd")]
        [AllowAnonymous]
        public IActionResult GetUserByEmailAndPwd(MstUserInfo user)
        {
            var _user = _userService.Login(user.UserEmail, MD5.Encrypt(user.Password));
            
            if (_user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(_user);
        }

        /// <summary>
        /// ログアウト処理
        /// </summary>
        /// <param name="id">ログアウトユーザのメールアドレス</param>
        /// <returns></returns>
        [HttpGet("logout")]
        [AllowAnonymous]
        public async Task<bool> Logout(string id)
        {
            return await _userService.Logout(id);

        }
        #endregion
    }
}
