using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Poker
{
    public partial class frmPoker : Form
    {
        #region 欄位
        /// <summary>
        /// 用來存放牌桌上五張牌的 PictureBox 陣列
        /// </summary>
        PictureBox[] pic = new PictureBox[5];

        /// <summary>
        /// 所有的牌的編號，從 0 到 51，對應到 52 張牌
        /// </summary>
        int[] allPoker = new int[52];

        /// <summary>
        /// 記錄玩家手牌的編號，從 0 到 51，對應到 52 張牌
        /// </summary>
        int[] playerPoker = new int[5];

        /// <summary>
        /// 宣告全域變數來追蹤資金與當前押注
        /// </summary>
        int totalFunds = 1000000;
        int currentBet = 0;

        #endregion

        public frmPoker()
        {
            InitializeComponent();
            InitializePoker();
        }

        #region 自定義方法
        private void InitializePoker()
        {
            for (int i = 0; i < pic.Length; i++)
            {
                pic[i] = new PictureBox();
                pic[i].Image = GetImage("back");
                pic[i].Name = "pic" + i;
                pic[i].SizeMode = PictureBoxSizeMode.AutoSize;
                pic[i].Top = 30;
                pic[i].Left = 10 + ((pic[i].Width + 10) * i);
                // 預設牌桌上的牌不可點擊
                pic[i].Enabled = false;
                // 預設牌桌上的牌的 Tag 為 "back"，表示牌面朝下
                pic[i].Tag = "back";
                pic[i].Visible = true;

                // 將 pic 丟至到 grpPorker 內
                this.grpPoker.Controls.Add(pic[i]);

                pic[i].Click += Pic_Click;
            }
        }

        /// <summary>
        /// 顯示五張撲克牌到桌面上
        /// </summary>
        private void ShowCards()
        {
            for (int i = 0; i < playerPoker.Length; i++)
            {
                pic[i].Image = this.GetImage($"pic{playerPoker[i] + 1}");
            }
        }

        /// <summary>
        /// 取得圖片資源
        /// </summary>
        /// <param name="name">string 的牌名 </param>
        /// <returns></returns>
        private Image GetImage(string name)
        {
            return Properties.Resources.ResourceManager.GetObject(name) as Image;
        }

        /// <summary>
        /// 取得圖片資源
        /// </summary>
        /// <param name="num">撲克牌編號</param>
        /// <returns></returns>
        private Image GetImage(int num)
        {
            return GetImage($"pic{num}");
        }

        /// <summary>
        /// 將 allPoker 陣列中的牌隨機打亂，模擬洗牌的過程
        /// </summary>
        private void Shuffle()
        {
            Random rand = new Random();
            for (int i = 0; i < 1000; i++)
            {
                int r = rand.Next(allPoker.Length);
                int temp = allPoker[r];
                allPoker[r] = allPoker[0];
                allPoker[0] = temp;
            }
        }

        /// <summary>
        /// 取得牌型對應的賠率
        /// </summary>
        /// <param name="handType">牌型名稱</param>
        /// <returns>賠率</returns>
        private int GetOdds(string handType)
        {
            int odds = 0;
            switch (handType)
            {
                case "皇家同花順": odds = 250; break;
                case "同花順": odds = 50; break;
                case "四條": odds = 25; break;
                case "葫蘆": odds = 9; break;
                case "同花": odds = 6; break;
                case "順子": odds = 4; break;
                case "三條": odds = 3; break;
                case "兩對": odds = 2; break;
                case "一對": odds = 1; break;
                default: odds = 0; break; // 沒中獎
            }
            return odds;
        }

        #endregion

        #region 事件處理程序

        /// <summary>
        /// 牌桌上的牌被按下時，顯示訊息框告訴使用者按下了哪一張牌
        /// </summary>
        private void Pic_Click(object sender, EventArgs e)
        {
            PictureBox pic = sender as PictureBox;

            int index = int.Parse(pic.Name.Replace("pic", ""));

            int cardNum = playerPoker[index] + 1;

            // 如果牌面朝下，則翻開牌面；如果牌面朝上，則翻回背面
            if (pic.Tag.ToString() == "back")
            {
                pic.Tag = "front";
                pic.Image = GetImage(cardNum);
            }
            else
            {
                pic.Tag = "back";
                pic.Image = GetImage("back");
            }
        }

        /// <summary>
        /// 當按下發牌按鈕時，隨機產生五個1~52的數字，並將對應的圖片顯示在牌桌上
        /// </summary>
        private async void btnDealCard_Click(object sender, EventArgs e)
        {
            // 防呆：如果還沒押注，提醒玩家先押注
            if (currentBet == 0)
            {
                MessageBox.Show("請先設定押注金額並按下押注按鈕！");
                return;
            }

            // 將上一把玩的結果清除
            this.lblResult.Text = "";

            // 將牌桌上的牌重置為背面圖
            for (int i = 0; i < pic.Length; i++)
            {
                pic[i].Image = GetImage("back");
            }

            // 將所有牌的編號從 0 到 51 填入 allPoker 陣列
            for (int i = 0; i < allPoker.Length; i++)
            {
                allPoker[i] = i;
            }

            // 洗牌
            this.Shuffle();

            // 暫停500ms
            await Task.Delay(500);

            // 發前五張牌給玩家，並將對應的牌面圖顯示在牌桌上
            for (int i = 0; i < playerPoker.Length; i++)
            {
                // 取前52張牌的前五張牌
                playerPoker[i] = allPoker[i];
            }

            // 將對應的牌面圖顯示在牌桌上
            this.ShowCards();

            // 啟用所有牌的點擊事件
            for (int i = 0; i < pic.Length; i++)
            {
                // 將牌桌上的牌設成可以點擊
                pic[i].Enabled = true;
                // 將牌桌上的牌的 Tag 設成 "front"，表示牌面朝上
                pic[i].Tag = "front";
            }

            // 啟用換牌按鈕
            btnChangeCard.Enabled = true;
            btnDealCard.Enabled = false;
        }

        /// <summary>
        /// 當按下換牌按鈕時，將玩家手牌中被選中的牌換成新的牌，並將對應的圖片顯示在牌桌上
        /// </summary>
        private void btnChangeCard_Click(object sender, EventArgs e)
        {
            int startIndex = 5; // 從 allPoker 陣列的第 5 張牌開始換牌，因為前 5 張牌已經發給玩家了

            for (int i = 0; i < playerPoker.Length; i++)
            {
                // 如果牌面朝下，表示玩家選擇換掉這張牌
                if (pic[i].Tag.ToString() == "back")
                {
                    // 將玩家手牌中被選中的牌換成新的牌
                    playerPoker[i] = allPoker[startIndex];
                    // 將對應的牌面圖顯示在牌桌上
                    pic[i].Image = GetImage(playerPoker[i] + 1);
                    pic[i].Tag = "front";

                    startIndex++;
                }
            }

            for (int i = 0; i < pic.Length; i++)
            {
                // 將牌桌上的牌設成不可點擊
                pic[i].Enabled = false;
            }

            // 將換牌按鈕設成不可用，表示玩家已經完成換牌了
            this.btnChangeCard.Enabled = false;

            // 將判斷牌型的按鈕設成可用，表示玩家可以開始判斷牌型了
            this.btnCheck.Enabled = true;
        }

        /// <summary>
        /// 當按下判斷牌型按鈕時，根據玩家手牌的編號，判斷玩家的牌型，並結算獎金
        /// </summary>
        private void btnCheck_Click(object sender, EventArgs e)
        {
            string[] colorList = { "梅花", "方塊", "愛心", "黑桃" };
            string[] pointList = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

            // 計錄目前五張撲克牌的花色和點數的陣列
            int[] pokerColor = new int[5];
            int[] pokerPoint = new int[5];

            for (int i = 0; i < playerPoker.Length; i++)
            {
                pokerColor[i] = playerPoker[i] % 4;
                pokerPoint[i] = playerPoker[i] / 4;
            }

            // 記錄花色和點數出現次數的陣列
            int[] colorCount = new int[4];
            int[] pointCount = new int[13];

            // 統計 color 和 point 出現次數
            for (int i = 0; i < pokerColor.Length; i++)
            {
                int color = pokerColor[i];
                int point = pokerPoint[i];

                colorCount[color]++;
                pointCount[point]++;
            }

            Array.Sort(colorCount, colorList);
            Array.Reverse(colorCount);
            Array.Reverse(colorList);

            Array.Sort(pointCount, pointList);
            Array.Reverse(pointCount);
            Array.Reverse(pointList);

            // 判斷牌型邏輯
            bool isFlush = (colorCount[0] == 5);
            bool isSingle = (pointCount[0] == 1 && pointCount[1] == 1 && pointCount[2] == 1 && pointCount[3] == 1 && pointCount[4] == 1);
            bool isDiffFout = (pokerPoint.Max() - pokerPoint.Min() == 4);
            bool isRoyal = pokerPoint.Contains(0) && pokerPoint.Contains(9) && pokerPoint.Contains(10) && pokerPoint.Contains(11) && pokerPoint.Contains(12);
            bool isRoyalisFlush = isFlush && isRoyal;
            bool isStraightFlush = isFlush && isSingle && isDiffFout;
            bool isStraight = isSingle && (isDiffFout || isRoyal);
            bool isFourOfAKind = (pointCount[0] == 4);
            bool isFullHouse = (pointCount[0] == 3 && pointCount[1] == 2);
            bool isThreeOfAKind = (pointCount[0] == 3 && pointCount[1] == 1);
            bool isTwoPair = (pointCount[0] == 2 && pointCount[1] == 2);
            bool isOnePair = (pointCount[0] == 2 && pointCount[1] == 1);

            string result = "";
            string pureHandType = ""; // 專門傳給 GetOdds 算賠率用的字串

            if (isRoyalisFlush)
            {
                result = $"{colorList[0]} 皇家同花順";
                pureHandType = "皇家同花順";
            }
            else if (isStraightFlush)
            {
                result = $"{colorList[0]} 同花順";
                pureHandType = "同花順";
            }
            else if (isStraight)
            {
                result = "順子";
                pureHandType = "順子";
            }
            else if (isFourOfAKind)
            {
                result = $"{pointList[0]} 四條";
                pureHandType = "四條";
            }
            else if (isFullHouse)
            {
                result = $"{pointList[0]}三張{pointList[1]}兩張 葫蘆";
                pureHandType = "葫蘆";
            }
            else if (isFlush)
            {
                result = $"{colorList[0]} 同花";
                pureHandType = "同花";
            }
            else if (isThreeOfAKind)
            {
                result = $"{pointList[0]} 三條";
                pureHandType = "三條";
            }
            else if (isTwoPair)
            {
                result = $"{pointList[0]},{pointList[1]} 兩對";
                pureHandType = "兩對";
            }
            else if (isOnePair)
            {
                result = $"{pointList[0]} 一對";
                pureHandType = "一對";
            }
            else
            {
                result = "雜牌";
                pureHandType = "雜牌";
            }

            lblResult.Text = result;
            btnChangeCard.Enabled = false;
            btnCheck.Enabled = false;
            btnDealCard.Enabled = true;

            // ================= 結算獎金 =================
            int odds = GetOdds(pureHandType);
            int winnings = currentBet * odds;

            if (winnings > 0)
            {
                totalFunds += winnings;
                MessageBox.Show($"牌型：{pureHandType}\n賠率：{odds} 倍\n恭喜中獎！獲得 {winnings} 元！");
            }
            else
            {
                MessageBox.Show("沒中獎，再接再厲！");
            }

            // 更新介面資金，重置押注狀態
            if (txtTotalFunds != null)
            {
                txtTotalFunds.Text = totalFunds.ToString();
            }

            btnBet.Enabled = true;
            currentBet = 0; // 重置本局押注金額
            // ============================================
        }

        /// <summary>
        /// 當表單被按下鍵盤時觸發 (作弊鍵)
        /// </summary>
        private void frmPoker_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (this.btnDealCard.Enabled == false)
            {
                switch (e.KeyChar)
                {
                    case 'q':
                        // 皇家同花順
                        playerPoker[0] = 51;
                        playerPoker[1] = 47;
                        playerPoker[2] = 43;
                        playerPoker[3] = 39;
                        playerPoker[4] = 3;
                        break;
                    case 'w':
                        // 同花順
                        playerPoker[0] = 37;
                        playerPoker[1] = 33;
                        playerPoker[2] = 29;
                        playerPoker[3] = 25;
                        playerPoker[4] = 21;
                        break;
                    case 'e':
                        // 同花
                        playerPoker[0] = 50;
                        playerPoker[1] = 38;
                        playerPoker[2] = 34;
                        playerPoker[3] = 22;
                        playerPoker[4] = 18;
                        break;
                    case 'r':
                        // 四條 (鐵支)
                        playerPoker[0] = 48;
                        playerPoker[1] = 39;
                        playerPoker[2] = 38;
                        playerPoker[3] = 37;
                        playerPoker[4] = 36;
                        break;
                    case 't':
                        // 葫蘆
                        playerPoker[0] = 30;
                        playerPoker[1] = 29;
                        playerPoker[2] = 6;
                        playerPoker[3] = 5;
                        playerPoker[4] = 4;
                        break;
                    case 'y':
                        // 三條
                        playerPoker[0] = 48;
                        playerPoker[1] = 39;
                        playerPoker[2] = 15;
                        playerPoker[3] = 14;
                        playerPoker[4] = 13;
                        break;
                }

                // 顯示五張撲克牌到桌面上
                this.ShowCards();
            }
        }

        private void btnBet_Click(object sender, EventArgs e)
        {
            // 取得玩家輸入的押注金額
            if (int.TryParse(txtBetAmount.Text, out currentBet))
            {
                if (currentBet > 0 && currentBet <= totalFunds)
                {
                    // 扣除總資金並更新介面
                    totalFunds -= currentBet;
                    txtTotalFunds.Text = totalFunds.ToString();

                    // 鎖定「押注」按鈕，避免重複押注
                    btnBet.Enabled = false;
                }
                else
                {
                    MessageBox.Show("資金不足或輸入金額無效！");
                    currentBet = 0;
                }
            }
            else
            {
                MessageBox.Show("請輸入正確的數字格式！");
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
        }

        #endregion
    }
}