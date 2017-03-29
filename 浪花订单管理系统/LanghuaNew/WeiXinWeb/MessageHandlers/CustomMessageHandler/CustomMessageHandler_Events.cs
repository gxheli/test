/*----------------------------------------------------------------
    Copyright (C) 2016 Senparc
    
    文件名：CustomMessageHandler_Events.cs
    文件功能描述：自定义MessageHandler
    
    
    创建标识：Senparc - 20150312
----------------------------------------------------------------*/

using System;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Senparc.Weixin.MP.Agent;
using Senparc.Weixin.Context;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.MP.Sample.CommonService.Download;
using Senparc.Weixin.MP.Sample.CommonService.Utilities;
using WXData;
using System.Collections.Generic;
using Simple.OData.Client;
using System.Configuration;
using LanghuaNew.Data;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Commond;
using Senparc.Weixin.MP.AdvancedAPIs;
using Newtonsoft.Json.Linq;
using Senparc.Weixin.MP.CommonAPIs;

namespace Senparc.Weixin.MP.Sample.CommonService.CustomMessageHandler
{
    /// <summary>
    /// 自定义MessageHandler
    /// </summary>
    public partial class CustomMessageHandler
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        private string GetWelcomeInfo()
        {
            return "感谢关注浪花朵朵公众号！如您曾在浪花朵朵预约系统中填写过订单资料，可通过菜单绑定预约系统，查看订单和确认单。";
        }

        public string GetDownloadInfo(CodeRecord codeRecord)
        {

            return string.Format(@"您已通过二维码验证，浏览器即将开始下载 Senparc.Weixin SDK 帮助文档。
当前选择的版本：v{0}

我们期待您的意见和建议，客服热线：400-031-8816。

感谢您对盛派网络的支持！

© 2016 Senparc", codeRecord.Version);
        }

        public override IResponseMessageBase OnTextOrEventRequest(RequestMessageText requestMessage)
        {
            // 预处理文字或事件类型请求。
            // 这个请求是一个比较特殊的请求，通常用于统一处理来自文字或菜单按钮的同一个执行逻辑，
            // 会在执行OnTextRequest或OnEventRequest之前触发，具有以下一些特征：
            // 1、如果返回null，则继续执行OnTextRequest或OnEventRequest
            // 2、如果返回不为null，则终止执行OnTextRequest或OnEventRequest，返回最终ResponseMessage
            // 3、如果是事件，则会将RequestMessageEvent自动转为RequestMessageText类型，其中RequestMessageText.Content就是RequestMessageEvent.EventKey

            if (requestMessage.Content == "OneClick")
            {
                var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                strongResponseMessage.Content = "您点击了底部按钮。\r\n为了测试微信软件换行bug的应对措施，这里做了一个——\r\n换行";
                return strongResponseMessage;
            }
            return null;//返回null，则继续执行OnTextRequest或OnEventRequest
        }

        public override IResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        {
            IResponseMessageBase reponseMessage = null;
            MenuItem ItemObj = JsonConvert.DeserializeObject<MenuItem>(HttpHelper.GetActionForOdata("odata/MenuItems(" + requestMessage.EventKey + ")").Result);
            if (ItemObj != null)
            {
                //图文类
                if (ItemObj.ItemType == MenuType.PictrueMenu)
                {
                    var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
                    reponseMessage = strongResponseMessage;
                    LHNew lhnew = JsonConvert.DeserializeObject<LHNew>(HttpHelper.GetActionForOdata("odata/LHNews(" + ItemObj.value + ")?$expand=Articles").Result);
                    //LHNew lhnew = Task.Run(async () =>
                    //{
                    //   return await client.For<LHNew>().Key(ItemObj.value).Expand(P => P.Articles).FindEntryAsync();
                    //}).Result;

                    if (lhnew != null)
                    {
                        foreach (var article in lhnew.Articles)
                        {
                            strongResponseMessage.Articles.Add(new Article()
                            {
                                Title = article.Title,
                                Description = article.Description,
                                PicUrl = article.PicUrl,
                                Url = article.Url
                            });
                        }
                    }
                }
                //文本类
                else
                {
                    var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                    reponseMessage = strongResponseMessage;
                    strongResponseMessage.Content = ItemObj.Text;
                }
                return reponseMessage;
            }


            //            string RequestType = requestMessage.EventKey.Split('_')[0];
            //            string RequestID = requestMessage.EventKey.Split('_')[1];

            //            if (RequestType == "News")
            //            {
            //                var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
            //                reponseMessage = strongResponseMessage;

            //                WeiXinContent content = new WeiXinContent();
            //                int ID = int.Parse(RequestID);
            //                LHNew lhnew = content.LHNews.Include("Articles").Where(p => p.LHNewID == ID).FirstOrDefault();
            //                if (lhnew != null)
            //                {
            //                  foreach(var article in lhnew.Articles)
            //                    {
            //                        strongResponseMessage.Articles.Add(new Article()
            //                        {
            //                            Title = article.Title,
            //                            Description = article.Description,
            //                            PicUrl = article.PicUrl,
            //                            Url = article.Url
            //                        });
            //                    } 
            //                }
            //                return reponseMessage;


            //            }
            //            else
            //            {
            //                var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
            //                reponseMessage = strongResponseMessage;
            //                if (RequestID == "1")
            //                {
            //                    strongResponseMessage.Content = @"私人定制QQ号：
            //2848213503
            //1310692387 

            //售前电话：
            //15818777250

            //售后电话：
            //18124516440 萍萍 
            //18124569281 朵朵 
            //                        ";
            //                }
            //                else if(RequestID == "2")
            //                {
            //                    strongResponseMessage.Content = "天猫店：http://szlhddgl.tmall.com \n 淘宝店：http://dodotour.taobao.com ";
            //                }
            //                else
            //                {
            //                    strongResponseMessage.Content = "请输入:绑定账号:#淘宝ID#,#电话号码#";
            //                }

            //                return reponseMessage;
            //            }
            ////菜单点击，需要跟创建菜单时的Key匹配
            //switch (requestMessage.EventKey)
            //{
            //    case "OneClick":
            //        {
            //            //这个过程实际已经在OnTextOrEventRequest中完成，这里不会执行到。
            //            var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
            //            reponseMessage = strongResponseMessage;
            //            strongResponseMessage.Content = "您点击了底部按钮。\r\n为了测试微信软件换行bug的应对措施，这里做了一个——\r\n换行";
            //        }
            //        break;
            //    case "SubClickRoot_Text":
            //        {
            //            var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
            //            reponseMessage = strongResponseMessage;
            //            strongResponseMessage.Content = "您点击了子菜单按钮。";
            //        }
            //        break;
            //    case "SubClickRoot_News":
            //        {
            //            var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
            //            reponseMessage = strongResponseMessage;
            //            strongResponseMessage.Articles.Add(new Article()
            //            {
            //                Title = "您点击了子菜单图文按钮",
            //                Description = "您点击了子菜单图文按钮，这是一条图文信息。",
            //                PicUrl = "http://sdk.weixin.senparc.com/Images/qrcode.jpg",
            //                Url = "http://sdk.weixin.senparc.com"
            //            });
            //        }
            //        break;
            //    case "SubClickRoot_Music":
            //        {
            //            //上传缩略图
            //            var accessToken = CommonAPIs.AccessTokenContainer.TryGetAccessToken(appId, appSecret);
            //            var uploadResult = AdvancedAPIs.MediaApi.UploadTemporaryMedia(accessToken, UploadMediaFileType.thumb,
            //                                                         Server.GetMapPath("~/Images/Logo.jpg"));
            //            //设置音乐信息
            //            var strongResponseMessage = CreateResponseMessage<ResponseMessageMusic>();
            //            reponseMessage = strongResponseMessage;
            //            strongResponseMessage.Music.Title = "天籁之音";
            //            strongResponseMessage.Music.Description = "真的是天籁之音";
            //            strongResponseMessage.Music.MusicUrl = "http://sdk.weixin.senparc.com/Content/music1.mp3";
            //            strongResponseMessage.Music.HQMusicUrl = "http://sdk.weixin.senparc.com/Content/music1.mp3";
            //            strongResponseMessage.Music.ThumbMediaId = uploadResult.thumb_media_id;
            //        }
            //        break;
            //    case "SubClickRoot_Image":
            //        {
            //            //上传图片
            //            var accessToken = CommonAPIs.AccessTokenContainer.TryGetAccessToken(appId, appSecret);
            //            var uploadResult = AdvancedAPIs.MediaApi.UploadTemporaryMedia(accessToken, UploadMediaFileType.image,
            //                                                         Server.GetMapPath("~/Images/Logo.jpg"));
            //            //设置图片信息
            //            var strongResponseMessage = CreateResponseMessage<ResponseMessageImage>();
            //            reponseMessage = strongResponseMessage;
            //            strongResponseMessage.Image.MediaId = uploadResult.media_id;
            //        }
            //        break;
            //    case "SubClickRoot_Agent"://代理消息
            //        {
            //            //获取返回的XML
            //            DateTime dt1 = DateTime.Now;
            //            reponseMessage = MessageAgent.RequestResponseMessage(this, agentUrl, agentToken, RequestDocument.ToString());
            //            //上面的方法也可以使用扩展方法：this.RequestResponseMessage(this,agentUrl, agentToken, RequestDocument.ToString());

            //            DateTime dt2 = DateTime.Now;

            //            if (reponseMessage is ResponseMessageNews)
            //            {
            //                (reponseMessage as ResponseMessageNews)
            //                    .Articles[0]
            //                    .Description += string.Format("\r\n\r\n代理过程总耗时：{0}毫秒", (dt2 - dt1).Milliseconds);
            //            }
            //        }
            //        break;
            //    case "Member"://托管代理会员信息
            //        {
            //            //原始方法为：MessageAgent.RequestXml(this,agentUrl, agentToken, RequestDocument.ToString());//获取返回的XML
            //            reponseMessage = this.RequestResponseMessage(agentUrl, agentToken, RequestDocument.ToString());
            //        }
            //        break;
            //    case "OAuth"://OAuth授权测试
            //        {
            //            var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
            //            strongResponseMessage.Articles.Add(new Article()
            //            {
            //                Title = "OAuth2.0测试",
            //                Description = "点击【查看全文】进入授权页面。\r\n注意：此页面仅供测试（是专门的一个临时测试账号的授权，并非Senparc.Weixin.MP SDK官方账号，所以如果授权后出现错误页面数正常情况），测试号随时可能过期。请将此DEMO部署到您自己的服务器上，并使用自己的appid和secret。",
            //                Url = "http://sdk.weixin.senparc.com/oauth2",
            //                PicUrl = "http://sdk.weixin.senparc.com/Images/qrcode.jpg"
            //            });
            //            reponseMessage = strongResponseMessage;
            //        }
            //        break;
            //    case "Description":
            //        {
            //            var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
            //            strongResponseMessage.Content = GetWelcomeInfo();
            //            reponseMessage = strongResponseMessage;
            //        }
            //        break;
            //    case "SubClickRoot_PicPhotoOrAlbum":
            //        {
            //            var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
            //            reponseMessage = strongResponseMessage;
            //            strongResponseMessage.Content = "您点击了【微信拍照】按钮。系统将会弹出拍照或者相册发图。";
            //        }
            //        break;
            //    case "SubClickRoot_ScancodePush":
            //        {
            //            var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
            //            reponseMessage = strongResponseMessage;
            //            strongResponseMessage.Content = "您点击了【微信扫码】按钮。";
            //        }
            //        break;
            //    case "ConditionalMenu_Male":
            //        {
            //            var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
            //            reponseMessage = strongResponseMessage;
            //            strongResponseMessage.Content = "您点击了个性化菜单按钮，您的微信性别设置为：男。";
            //        }
            //        break;
            //    case "ConditionalMenu_Femle":
            //        {
            //            var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
            //            reponseMessage = strongResponseMessage;
            //            strongResponseMessage.Content = "您点击了个性化菜单按钮，您的微信性别设置为：女。";
            //        }
            //        break;
            //    default:
            //        {
            //            var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
            //            strongResponseMessage.Content = "您点击了按钮，EventKey：" + requestMessage.EventKey;
            //            reponseMessage = strongResponseMessage;
            //        }
            //        break;
            //}

            return reponseMessage;
        }

        public override IResponseMessageBase OnEvent_EnterRequest(RequestMessageEvent_Enter requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            responseMessage.Content = "您刚才发送了ENTER事件请求。";
            return responseMessage;
        }

        public override IResponseMessageBase OnEvent_LocationRequest(RequestMessageEvent_Location requestMessage)
        {
            //这里是微信客户端（通过微信服务器）自动发送过来的位置信息
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "这里写什么都无所谓，比如：上帝爱你！";
            return responseMessage;//这里也可以返回null（需要注意写日志时候null的问题）
        }

        public override IResponseMessageBase OnEvent_ScanRequest(RequestMessageEvent_Scan requestMessage)
        {
            //通过扫描
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            var OpenID = requestMessage.FromUserName;
            string message = "欢迎关注浪花朵朵旅游";

            //EventKey值包含ID和类型，原客户扫码绑定只包含ID，增加供应商扫码绑定功能，后面加类型用于区分，多个参数用“,”隔开 ---2016-09-22 by何力
            string id = requestMessage.EventKey;
            if (id.Length > 4 && id.Substring(id.Length - 4) == "8888")//客户
            {
                int CustomerID = int.Parse(id.Substring(0, id.Length - 4));
                Customer customer = JsonConvert.DeserializeObject<Customer>(HttpHelper.GetActionForOdata("odata/Customers(" + CustomerID + ")").Result);
                JObject open = JsonConvert.DeserializeObject<JObject>(HttpHelper.GetActionForOdata("odata/Customers?$filter=OpenID eq '" + OpenID + "'").Result);
                string str = open["value"].ToString();
                List<Customer> Customers = JsonConvert.DeserializeObject<List<Customer>>(str);
                Customer opencustomer = (Customers != null && Customers.Count > 0) ? Customers[0] : null;

                if (opencustomer != null && opencustomer.CustomerID > 0 && customer.OpenID != OpenID)
                {
                    message = "一个微信只能绑定一个淘宝ID，当前微信已与淘宝ID（" + opencustomer.CustomerTBCode + "）绑定。如需更换绑定，请用该淘宝ID联系旺旺客服进行解绑后再重新绑定。";
                }
                else
                {
                    if (string.IsNullOrEmpty(customer.OpenID))
                    {
                        Task.Run(async () =>
                        {
                            //绑定OPENID
                            //var bindCustomer = await client.For<Customer>().Key(CustomerID).FindEntryAsync();
                            customer.OpenID = OpenID;
                            await client.For<Customer>().Key(CustomerID).Set(customer).UpdateEntryAsync();
                            //为用户设置备注
                            //WeiXinHelper.UpdateUserRemark(OpenID, customer.CustomerTBCode);
                            UserApi.UpdateRemark(AccessTokenContainer.GetFirstOrDefaultAppId(), OpenID, customer.CustomerTBCode);
                        });
                        message = "您的微信已经与浪花朵朵预约系统绑定成功，通过菜单可直接查看您的订单。";
                    }
                    else
                    {
                        if (customer.OpenID == OpenID)
                        {
                            message = "您的微信已经与浪花朵朵预约系统绑定，无需重复绑定。如需解绑，请联系旺旺客服。";
                        }
                        else
                        {
                            message = "您的淘宝ID已与其它微信进行绑定，如需绑定新的微信，请用该淘宝ID联系旺旺客服进行解绑后再重新绑定。";
                        }
                    }
                }
            }
            if (id.Length > 4 && id.Substring(id.Length - 4) == "9999")//供应商
            {
                int SupplierUserID = int.Parse(id.Substring(0, id.Length - 4));

                SupplierUser user = JsonConvert.DeserializeObject<SupplierUser>(HttpHelper.GetActionForOdata("odata/SupplierUsers(" + SupplierUserID + ")").Result);
                JObject open = JsonConvert.DeserializeObject<JObject>(HttpHelper.GetActionForOdata("odata/SupplierUsers?$filter=OpenID eq '" + OpenID + "'").Result);
                string str = open["value"].ToString();
                List<SupplierUser> users = JsonConvert.DeserializeObject<List<SupplierUser>>(str);
                SupplierUser openuser = (users != null && users.Count > 0) ? users[0] : null;

                if (openuser != null && openuser.SupplierID > 0 && openuser.OpenID != OpenID)
                {
                    message = "一个微信只能绑定一个账号，当前微信已与账号（" + openuser.SupplierUserName + "）绑定。如需更换绑定，请先进行解绑后再重新绑定。";
                }
                else
                {
                    if (string.IsNullOrEmpty(user.OpenID))
                    {
                        Task.Run(async () =>
                        {
                            //绑定OPENID
                            //var bindCustomer = await client.For<Customer>().Key(CustomerID).FindEntryAsync();
                            user.OpenID = OpenID;
                            await client.For<SupplierUser>().Key(SupplierUserID).Set(user).UpdateEntryAsync();
                            //为用户设置备注
                            UserApi.UpdateRemark(AccessTokenContainer.GetFirstOrDefaultAppId(), OpenID, "(供应商)" + user.SupplierUserName);
                        });
                        message = "您的微信已经与浪花朵朵订单管理系统绑定成功，通过消息配置后可收到订单提醒。";
                    }
                    else
                    {
                        if (user.OpenID == OpenID)
                        {
                            message = "您的微信已经与浪花朵朵订单管理系统绑定，无需重复绑定。";
                        }
                        else
                        {
                            message = "您的账号已与其它微信进行绑定，如需绑定新的微信，请先进行解绑后再重新绑定。";
                        }
                    }
                }
            }
            responseMessage.Content = string.Format(message);
            return responseMessage;
        }

        public override IResponseMessageBase OnEvent_ViewRequest(RequestMessageEvent_View requestMessage)
        {
            //说明：这条消息只作为接收，下面的responseMessage到达不了客户端，类似OnEvent_UnsubscribeRequest
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您点击了view按钮，将打开网页：" + requestMessage.EventKey;
            return responseMessage;
        }

        public override IResponseMessageBase OnEvent_MassSendJobFinishRequest(RequestMessageEvent_MassSendJobFinish requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "接收到了群发完成的信息。";
            return responseMessage;
        }

        /// <summary>
        /// 订阅（关注）事件
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            var OpenID = requestMessage.FromUserName;
            string message = GetWelcomeInfo();
            //扫码关注
            if (!string.IsNullOrEmpty(requestMessage.EventKey) && requestMessage.EventKey.Split('_')[0] == "qrscene")
            {
                //EventKey值包含ID和类型，原客户扫码绑定只包含ID，增加供应商扫码绑定功能，后面加类型用于区分，多个参数用“,”隔开 ---2016-09-22 by何力
                string id = requestMessage.EventKey.Split('_')[1];
                if (id.Length > 4 && id.Substring(id.Length - 4) == "8888")//客户
                {
                    int CustomerID = int.Parse(id.Substring(0, id.Length - 4));
                    Customer customer = JsonConvert.DeserializeObject<Customer>(HttpHelper.GetActionForOdata("odata/Customers(" + CustomerID + ")").Result);
                    JObject open = JsonConvert.DeserializeObject<JObject>(HttpHelper.GetActionForOdata("odata/Customers?$filter=OpenID eq '" + OpenID + "'").Result);
                    string str = open["value"].ToString();
                    List<Customer> Customers = JsonConvert.DeserializeObject<List<Customer>>(str);
                    Customer opencustomer = (Customers != null && Customers.Count > 0) ? Customers[0] : null;

                    if (opencustomer != null && opencustomer.CustomerID > 0 && customer.OpenID != OpenID)
                    {
                        message = "感谢关注浪花朵朵公众号！一个微信只能绑定一个淘宝ID，当前微信已与淘宝ID（" + opencustomer.CustomerTBCode + "）绑定。如需更换绑定，请用该淘宝ID联系旺旺客服进行解绑后再重新绑定。";
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(customer.OpenID))
                        {
                            Task.Run(async () =>
                            {
                                //绑定OPENID
                                //var bindCustomer = await client.For<Customer>().Key(CustomerID).FindEntryAsync();
                                customer.OpenID = OpenID;
                                await client.For<Customer>().Key(CustomerID).Set(customer).UpdateEntryAsync();
                                //为用户设置备注
                                UserApi.UpdateRemark(AccessTokenContainer.GetFirstOrDefaultAppId(), OpenID, customer.CustomerTBCode);
                            });
                            message = "感谢关注浪花朵朵公众号！您的微信已经与浪花朵朵预约系统绑定成功，通过菜单可直接查看您的订单。";
                        }
                        else
                        {
                            if (customer.OpenID == OpenID)
                            {
                                message = "感谢关注浪花朵朵公众号！您的微信已经与浪花朵朵预约系统绑定，无需重复绑定。如需解绑，请联系旺旺客服。";
                            }
                            else
                            {
                                message = "感谢关注浪花朵朵公众号！您的淘宝ID已与其它微信进行绑定，如需绑定新的微信，请用该淘宝ID联系旺旺客服进行解绑后再重新绑定。";
                            }
                        }
                    }
                }
                if (id.Length > 4 && id.Substring(id.Length - 4) == "9999")//供应商
                {
                    int SupplierUserID = int.Parse(id.Substring(0, id.Length - 4));

                    SupplierUser user = JsonConvert.DeserializeObject<SupplierUser>(HttpHelper.GetActionForOdata("odata/SupplierUsers(" + SupplierUserID + ")").Result);
                    JObject open = JsonConvert.DeserializeObject<JObject>(HttpHelper.GetActionForOdata("odata/SupplierUsers?$filter=OpenID eq '" + OpenID + "'").Result);
                    string str = open["value"].ToString();
                    List<SupplierUser> users = JsonConvert.DeserializeObject<List<SupplierUser>>(str);
                    SupplierUser openuser = (users != null && users.Count > 0) ? users[0] : null;

                    if (openuser != null && openuser.SupplierID > 0 && openuser.OpenID != OpenID)
                    {
                        message = "感谢关注浪花朵朵公众号！一个微信只能绑定一个账号，当前微信已与账号（" + openuser.SupplierUserName + "）绑定。如需更换绑定，请用该账号联系管理员进行解绑后再重新绑定。";
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(user.OpenID))
                        {
                            Task.Run(async () =>
                            {
                                //绑定OPENID
                                user.OpenID = OpenID;
                                await client.For<SupplierUser>().Key(SupplierUserID).Set(user).UpdateEntryAsync();
                                //为用户设置备注
                                UserApi.UpdateRemark(AccessTokenContainer.GetFirstOrDefaultAppId(), OpenID, "(供应商)" + user.SupplierUserName);
                            });
                            message = "感谢关注浪花朵朵公众号！您的微信已经与浪花朵朵订单管理系统绑定成功，通过消息配置后可收到订单提醒。";
                        }
                        else
                        {
                            if (user.OpenID == OpenID)
                            {
                                message = "感谢关注浪花朵朵公众号！您的微信已经与浪花朵朵订单管理系统绑定，无需重复绑定。如需解绑，请联系管理员。";
                            }
                            else
                            {
                                message = "感谢关注浪花朵朵公众号！您的账号已与其它微信进行绑定，如需绑定新的微信，请用该账号联系管理员进行解绑后再重新绑定。";
                            }
                        }
                    }
                }
            }
            responseMessage.Content = string.Format(message);
            return responseMessage;
        }

        /// <summary>
        /// 退订
        /// 实际上用户无法收到非订阅账号的消息，所以这里可以随便写。
        /// unsubscribe事件的意义在于及时删除网站应用中已经记录的OpenID绑定，消除冗余数据。并且关注用户流失的情况。
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_UnsubscribeRequest(RequestMessageEvent_Unsubscribe requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "有空再来";
            return responseMessage;
        }

        /// <summary>
        /// 事件之扫码推事件(scancode_push)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_ScancodePushRequest(RequestMessageEvent_Scancode_Push requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之扫码推事件";
            return responseMessage;
        }

        /// <summary>
        /// 事件之扫码推事件且弹出“消息接收中”提示框(scancode_waitmsg)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_ScancodeWaitmsgRequest(RequestMessageEvent_Scancode_Waitmsg requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之扫码推事件且弹出“消息接收中”提示框";
            return responseMessage;
        }

        /// <summary>
        /// 事件之弹出拍照或者相册发图（pic_photo_or_album）
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_PicPhotoOrAlbumRequest(RequestMessageEvent_Pic_Photo_Or_Album requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出拍照或者相册发图";
            return responseMessage;
        }

        /// <summary>
        /// 事件之弹出系统拍照发图(pic_sysphoto)
        /// 实际测试时发现微信并没有推送RequestMessageEvent_Pic_Sysphoto消息，只能接收到用户在微信中发送的图片消息。
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_PicSysphotoRequest(RequestMessageEvent_Pic_Sysphoto requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出系统拍照发图";
            return responseMessage;
        }

        /// <summary>
        /// 事件之弹出微信相册发图器(pic_weixin)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_PicWeixinRequest(RequestMessageEvent_Pic_Weixin requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出微信相册发图器";
            return responseMessage;

        }

        /// <summary>
        /// 事件之弹出地理位置选择器（location_select）
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_LocationSelectRequest(RequestMessageEvent_Location_Select requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出地理位置选择器";
            return responseMessage;
        }
    }
}