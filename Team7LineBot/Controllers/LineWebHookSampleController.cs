using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Team7LineBot.Models;
using Team7LineBot.Repositories;

namespace Team7LineBot.Controllers
{
    public class LineBotWebHookController : isRock.LineBot.LineWebHookControllerBase
    {
        const string channelAccessToken = "SJjQF/AKrpieVtV2untAf6fa/Jjg0cf66L0vAdbHf/PsOFeJPm4fVUpO4Cvt1XZMfXkirlERMB3aVvan+DxhXOqaytsCvAD4WGgkBu6y9PnA3nowYRPsqtKqZ1f0UAkCQIQ/CcKiIgILNa2QoeLEOgdB04t89/1O/w1cDnyilFU=";
        //const string AdminUserId= "!!!改成你的AdminUserId!!!";
        string helpMsg = "Hi, 我是客服機器人,目前功能有\n/最新商品\n/熱門商品\n/高價稀有商品\n/查詢商品 '商品關鍵字'";

        public readonly ProductRepository _repo;

        public LineBotWebHookController()
        {
            _repo = new ProductRepository();
        }

        [Route("api/Team7MVC")]
        [HttpPost]
        public IHttpActionResult POST()
        {
            //設定ChannelAccessToken(或抓取Web.Config)
            this.ChannelAccessToken = channelAccessToken;
            //取得Line Event(範例，只取第一個)
            var LineEvent = this.ReceivedMessage.events.FirstOrDefault();

            try
            {
                var msg = new isRock.LineBot.CarouselTemplate();
                List<Product> products;
                var actions = new List<isRock.LineBot.TemplateActionBase>();
                isRock.LineBot.Column Column;

                //配合Line verify 
                if (LineEvent.replyToken == "00000000000000000000000000000000") return Ok();
                //回覆訊息
                if (LineEvent.type == "message")
                {
                    if (LineEvent.message.type == "text") //收到文字
                    {
                        if (LineEvent.message.text.Trim() == "/help" || LineEvent.message.text.Trim() == "/說明")
                        {
                            this.ReplyMessage(LineEvent.replyToken, helpMsg);
                            return Ok();
                        }

                        if (LineEvent.message.text.Trim().StartsWith("/"))
                        {
                            if (LineEvent.message.text.Trim() == "/熱門商品")
                            {
                                products = _repo.GetHotProducts();

                                foreach (var item in products)
                                {

                                    actions = new List<isRock.LineBot.TemplateActionBase>();
                                    string uri = "https://team7mvc20190606035227.azurewebsites.net/Product/ProductDetail/" + item.ProductID;
                                    string ImgUri = "https://team7mvc20190606035227.azurewebsites.net/Assets/images/img_Products/" + item.ProductID + ".jpg";
                                    actions.Add(new isRock.LineBot.UriAction() { label = "前往商品頁", uri = new Uri(uri) });

                                    Column = new isRock.LineBot.Column
                                    {
                                        title = item.ProductName,
                                        text = "$ " + item.UnitPrice.ToString("0.00"),
                                        thumbnailImageUrl = new Uri(ImgUri),
                                        actions = actions
                                    };
                                    msg.columns.Add(Column);
                                }
                                this.PushMessage(LineEvent.source.userId, msg);
                            }
                            else if (LineEvent.message.text.Trim() == "/最新商品")
                            {
                                products = _repo.GetNewProducts();

                                foreach (var item in products)
                                {

                                    actions = new List<isRock.LineBot.TemplateActionBase>();
                                    string uri = "https://team7mvc20190606035227.azurewebsites.net/Product/ProductDetail/" + item.ProductID;
                                    string ImgUri = "https://team7mvc20190606035227.azurewebsites.net/Assets/images/img_Products/" + item.ProductID + ".jpg";
                                    actions.Add(new isRock.LineBot.UriAction() { label = "前往商品頁", uri = new Uri(uri) });

                                    Column = new isRock.LineBot.Column
                                    {
                                        title = item.ProductName,
                                        text = "$ " + item.UnitPrice.ToString("0.00"),
                                        thumbnailImageUrl = new Uri(ImgUri),
                                        actions = actions
                                    };
                                    msg.columns.Add(Column);
                                }
                                this.PushMessage(LineEvent.source.userId, msg);
                            }
                            else if (LineEvent.message.text.Trim() == "/高價稀有商品")
                            {
                                products = _repo.GetExpensiveProducts();

                                foreach (var item in products)
                                {

                                    actions = new List<isRock.LineBot.TemplateActionBase>();
                                    string uri = "https://team7mvc20190606035227.azurewebsites.net/Product/ProductDetail/" + item.ProductID;
                                    string ImgUri = "https://team7mvc20190606035227.azurewebsites.net/Assets/images/img_Products/" + item.ProductID + ".jpg";
                                    actions.Add(new isRock.LineBot.UriAction() { label = "前往商品頁", uri = new Uri(uri) });

                                    Column = new isRock.LineBot.Column
                                    {
                                        title = item.ProductName,
                                        text = "$ " + item.UnitPrice.ToString("0.00"),
                                        thumbnailImageUrl = new Uri(ImgUri),
                                        actions = actions
                                    };
                                    msg.columns.Add(Column);
                                }
                                this.PushMessage(LineEvent.source.userId, msg);
                            }
                            else if(LineEvent.message.text.Trim().Contains("/查詢商品"))
                            {
                                products = _repo.GetProducts(LineEvent.message.text.Trim().Substring(6));

                                if(products.Count!=0)
                                {
                                    foreach (var item in products)
                                    {

                                        actions = new List<isRock.LineBot.TemplateActionBase>();
                                        string uri = "https://team7mvc20190606035227.azurewebsites.net/Product/ProductDetail/" + item.ProductID;
                                        string ImgUri = "https://team7mvc20190606035227.azurewebsites.net/Assets/images/img_Products/" + item.ProductID + ".jpg";
                                        actions.Add(new isRock.LineBot.UriAction() { label = "前往商品頁", uri = new Uri(uri) });

                                        Column = new isRock.LineBot.Column
                                        {
                                            title = item.ProductName,
                                            text = "$ " + item.UnitPrice.ToString("0.00"),
                                            thumbnailImageUrl = new Uri(ImgUri),
                                            actions = actions
                                        };
                                        msg.columns.Add(Column);

                                    }

                                    this.PushMessage(LineEvent.source.userId, msg);
                                }
                                else
                                {
                                    this.PushMessage(LineEvent.source.userId, "不好意思，查不到此產品");
                                }
                                
                            }
                            else
                            {
                                this.ReplyMessage(LineEvent.replyToken, helpMsg);
                            }
                        }
                    }


                    if (LineEvent.message.type == "sticker") //收到貼圖
                        this.ReplyMessage(LineEvent.replyToken, 1, 2);
                }
                //response OK
                return Ok();
            }
            catch (Exception ex)
            {
                this.ReplyMessage(LineEvent.replyToken, "發生錯誤:\n" + ex.Message);
                //response OK
                return Ok();
            }
        }
    }
}
