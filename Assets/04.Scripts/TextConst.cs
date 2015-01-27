//#define OutFile
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;

public static class TextConst
{
	private static Dictionary<int, string[]> GameText = new Dictionary<int, string[]> ();
	#if OutFile
	private static StringBuilder sb = new StringBuilder();
	#endif

	private static void AddString(int key, string Text_TW, string Text_EN = ""){
		#if OutFile
		sb.Append(Text_TW.ToString());
		sb.AppendLine();
		#endif
//		if (GameText.ContainsKey (key)) {
//			GameText[key][Language.zh_TW.GetHashCode()] = Text_TW;
//			GameText[key][Language.en.GetHashCode()] = Text_EN;
//		}else{
//			string [] Data = new string[2];
//			Data[Language.zh_TW.GetHashCode()] = Text_TW;
//			Data[Language.en.GetHashCode()] = Text_EN;
//			GameText.Add(key, Data);
//		}
	}

	public static string S (int index)
	{
//		if(GameText.ContainsKey(index))
//			return GameText[index][TeamManager.LanguageKind.GetHashCode()];
//		else
			return "";
	} 
	
	public static void InitText(){
		AddString (0, "身為隊長，你立志成為像[99ff00]{0}[-]一樣偉大的[99ff00]{1}[-]。"
		           ,"As a captain, you aimed to be like a great [99ff00]{1}[-] like [99ff00]{0}[-].");
		AddString (1, "控球後衛","Point Guard"); 
		AddString (2, "得分後衛","Shooting Guard");  
		AddString (3, "小前鋒","Small Forward"); 
		AddString (4, "大前鋒","Power Forward"); 
		AddString (5, "中鋒","Center"); 
		AddString (6, "(隊長)","(Captain)"); 
		AddString (7, "抱歉，伺服器維護中，預計等待10-15分鐘，請稍後再登入遊戲。",
		           "Server maintenance. Please try again later."); 
		AddString (8, "點擊螢幕開始遊戲。","Touch screen to start game"); 
		AddString (9, "Facebook驗證中...","Facebook Account Verification");
		AddString (10, "Facebook登入完成。","Facebook login completed.");
		AddString (11, "伺服器登入完成。","Login successful."); 
		AddString (12, "球員資料下載完成。","Download successful.");
		AddString (13, "關卡資料下載完成。","Download successful.");
		AddString (14, "請更新至最新版本。","Please download the new version." );
		AddString (15, "擊敗難纏的對手『{0}』，灌爆他家的籃筐。","Beat the tough opponent 『{0}』, dunk his basket."); 
		AddString (23, "主動技{0} Lv{1}","Active Skill{0} Lv{1}"); 
		AddString (24, "被動技{0} Lv{1}","Passive Skill{0} Lv{1}");
		AddString (26, "球隊資金不足","Team funds are insufficient.");
		AddString (29, "隊長","Captain"); 
		AddString (32, "2分{0:0}％  3分{1:0}％  灌籃{2}  速度{3}  控球{4}  抄截{5}  火鍋{6}",
		           "2Point{0:0}％ 3Point{1:0}％ Dunk{2} Speed{3} Dribble{4} Steal{5} Block{6}"); 
		AddString (33, "連線逾時，請重新登入遊戲。","Connection fail. Please login again."); 
		AddString (34, "東區 ","Eastern Conference"); 
		AddString (35, "西區 ","Western Conference"); 
		AddString (36, "網路傳輸失敗","Network Transport Problem"); //title
		AddString (37, "裝置網路異常","Device Network Problem"); //title
		AddString (38, "系統維護","Server maintenance. Please try again later.");
		AddString (39, "網路傳輸失敗，請重新進行操作。","Network problem. \nPlease try again.");
		AddString (40, "知名度","Popularity");
		AddString (41, "距離{0:0} FG {1:0}%","Distance{0:0} FG {1:0}%");
		AddString (42, "距離{0:0} 3FG {1:0}%","Distance{0:0} 3FG {1:0}%");
		AddString (43, "您輸入的隊名已被使用，請輸入其他名稱。","This name had existed, please enter the another one.");
		AddString (56, "場次","Games Played"); 
		AddString (57, "前置關卡尚未通過，無法挑戰。","You haven't pass the previous stages."); 
		AddString (63, "問題物品","Data error.");

		AddString (72, "獲得『{0}』X1","Obtain『{0}』X1"); 
		AddString (73, "『{0}』","『{0}』");

		AddString (76, " 勝利 ", "WIN"); 
        AddString (77, " 敗北 ", "LOSE");
		AddString (79, "任務完成", "CLEAR"); 
		AddString (80, "任務失敗", "FAIL");
		AddString (81, "技能『{0}』發動機率{1}％","Skill『{0}』Launch Rate{1}％");
		AddString (82, "太棒了！『{0}』加入球隊！","Great! ！『{0}』 join your team！");
		AddString (83, "資料下載中...","Downloading");
        AddString (84, "即將連結FB。若你的FB帳號曾連結其他裝置中的籃球黑幫，[99ff00]前次記錄將覆蓋這個裝置上的遊戲記錄與儲值記錄[-]，確定繼續嗎？"
		           ,"Connecting to Facebook. If your FB account has connected to Gang of basketballers on other devices, [99ff00] the game and purchase records on the previous device will replace those on this device.[-] Do you want to continue?  ");
		AddString (85, "球隊等級", "Level");
		AddString (86, "先發戰力", "Starting Lineup\nForce");
		AddString (87, "對戰勝場", "PVP Wins"); 
		AddString (88, "星星數", "Stars");

		AddString (93, "裝置無連線能力，請檢查您的網路。","Your network connection is not good.\nPlease try again later.");
		AddString (94, "完成『{0}』。","Completed『{0}』."); 
		AddString (95, "尚未取得『{0}』。","Have not get『{0}』.");
		
		AddString (98, "請輸入球隊名稱。","Enter a team name");
		AddString (99, "球隊名稱最少2個字元，最多20個字元。","At least 2 characters, at most 20 characters."); 

		AddString (101, "角色資料錯誤請重新登入。","Network problem. Please login again.");

		AddString (103, "我方球隊","Our team"); 
		AddString (104, "敵方球隊","Opponent team");
		AddString (105, "＋{0}","＋{0}");
		AddString (106, "移動控球員教學完成。","Offensive Tutorial Completed");
		AddString (107, "移動防守球員教學完成。","Defensive Tutorial Completed");
		AddString (108, "灌籃教學完成。","Dunk Tutorial Completed");
		AddString (109, "投籃教學完成。","Shooting Tutorial Completed");
		AddString (110, "點擊頭像傳球教學完成。","Passing Tutorial Completed");
		AddString (111, "點擊球員傳球教學完成。","Passing Tutorial Completed");

		AddString (115, "聯合移動贈送您鑽石X{0}。","D {0} Complimentary diamonds for you.");
        AddString (116, "提升神奇徽章容量上限至[99ff00]{0}[-]，需要花費資金[99ff00]{1}[-]。是否確定提升？"
		           ,"Spend team fund [99ff00] {1} [-] to enlarge the magical badge capacity limit to [99ff00] {0} [-].");

		AddString (122, "      No.{0} {1}\n\n2分{2}%   3分{3}%\n灌籃{4}   速度{5}\n控球{6}   抄截{7}\n火鍋{8}\n\n技能 {9}",
		           "No.{0} {1}\n\n2Point{2}% 3Point{3}%\nDunk{4} Speed{5}\nDribble{6} Steal{7}\nBlock{8}\n\nSkill {9}");
		AddString (127, "購買球隊資金{0}","Buying {0} team funds.");
		AddString (128, "鑽石不夠，前往鑽石商店購買更多鑽石。","Your diamond is not enough, please go to diamond store to buy more.");
		AddString (130, "挑戰其他玩家？","Challenging other players?");
		AddString (131, "離開比賽？","Exit ");

		AddString (134, "消耗挑戰次數1，對『{0}』復仇。","Cost one chance to revenge yourself on『{0}』.");
        AddString (135, "0:00");
        AddString (136, "確定", "YES");
		AddString (137, "取消", "NO");
		AddString (138, "您的資料正送往雲端，除非太陽爆炸，否則我們將永久為您保存資料。","Save data in the cloud...");
		AddString (139, "提示","Hint");
		AddString (140, "跳投","Shoot");
		AddString (141, "傳球","Passing");
		AddString (142, "蓋火鍋","Block");
		AddString (143, "灌籃","Dunk");
		AddString (144, "移動球員","Moving");
		AddString (145, "出手","Shooting");
		AddString (146, "≤100", "≤100");
		AddString (147, ">100", ">100");
		AddString (148, ">400", ">400");
		AddString (149, ">1000", ">1000");
		AddString (150, ">1800", ">1800");
		AddString (154, "尋找對手","Search");
		AddString (155, "天梯挑戰","PVP");
        AddString (156, "獎勵兌換", "Awards");
		AddString (157, "對戰情報","Competitor information");
		AddString (158, "對手","Opponent"); 
		AddString (159, "比數","Scores");
		AddString (160, "復仇","Revenge");
		AddString (162, "先發","Starter");
		AddString (168, "主動技", "Active Skill");
		AddString (171, "球員訓練","Training");
		AddString (172, "更換球員","Change");
		AddString (173, "無{0}物品","none{0}item");
		AddString (174, "無{0}的球員","none{0}player");
		AddString (175, "目前沒有選擇項目","No object was chosen.");
		AddString (176, "裝備成功","Wear successfully");
		AddString (177, "換人成功","Change successfully");
		AddString (194, "- {0}","- {0}");
		AddString (195, "經驗值 {0}","EXP {0}");
        AddString (196, "獎金 {0}","Bonus Funds {0}");
		AddString (197, "裝備", "Equipment");
		AddString (198, "(數值隨機取得)","(Random Numerical)");
		AddString (199, "無物品ID:{0}的物品資訊","None ItemID:{0}");
		AddString (201, "已經先發","Already on the lineup");
		AddString (202, "已經裝備","Already wearing");
		AddString (203, "物品索引有誤","Item index error");
		AddString (204, "球員列表","Player List");
		AddString (205, "物品列表","Item List");
		AddString (206, "{0}[00ffff]+{1}[-]","{0}[00ffff]+{1}[-]");
		AddString (208, "已達訓練上限","Reaching the max level");
		AddString (209, "Lv[ff0000]{0}[-]","Lv[ff0000]{0}[-]");
		AddString (210, "該欄位已經開啟","This section was already open.");
		AddString (211, "是否花費{0}鑽石開啟欄位","Costing{0}diamond to get more training section?");

		AddString (218, "主場球衣","Primary Jersey");
		AddString (219, "球隊競賽時穿著。","Jersey");

		AddString (221, "{0:00}天{1:00}時", "{0:00}D{1:00}H");
		AddString (222, "{0:00}時{1:00}分", "{0:00}H{1:00}M");
		AddString (223, "{0:00}分{1:00}秒", "{0:00}M{1:00}S");
		AddString (225, "裝備已達強化上限","Reaching the max level.");
		AddString (226, "裝備強化成功","Upgrade successfully");
		AddString (227, "使用成功","Use successfully");
		AddString (228, "使用物品","Using an item.");
		AddString (229, "使用{0}顆鑽石或是消耗1鐵牛補給液恢復所有精力。",
		           "Using {0} diamond or 1 EnergyDrink to recover all energy.");
		AddString (230, "成長筆記不足","Notes are insufficient.");
		AddString (231, "精力全滿，快來一場吧！","Full Energy");
		AddString (232, "獲得資金：{0}","Obtaining funds：{0}");
		AddString (233, "冷卻時間未到","You haven't finished your cooldown time.");
		AddString (234, "{0}X{1}","{0}X{1}");
		AddString (235, "可抽1次", "Draw");
		AddString (236, "精力全滿","Full Energy");
		AddString (237, "確定覺醒","OK");
		AddString (238, "{0}+[ffff00]{1}[-]","{0}+[ffff00]{1}[-]");
		AddString (239, "[ff0000]{0}[-]","[ff0000]{0}[-]");
		AddString (240, "未達訓練等級上限","Not reach the max proper level.");
		AddString (241, "已達星等上限","Reaching the max level.");
		AddString (242, "{0}前","Before{0}");
		AddString (243, "購買神奇徽章上限","Spend team fund to enlarge the Magical Logo capacity.");
		AddString (244, "購買精力","Buy Energy.");
		AddString (245, "購買鑽石","Buy Diamond.");
		AddString (246, "購買球隊資金","Buy team funds");
		AddString (247, "對戰復仇","Revenge");
		AddString (248, "玩家對戰","PVP");
		AddString (249, "購買訓練空間","Buy more training field.");
		AddString (250, "Facebook連結","Connected with Facebook ");
		AddString (251, "感謝您首次購買鑽石，贈送『{0}』。","Thanks for your first purchase,  giving you away『{0}』.");
		AddString (252, "恭喜您達成與{0}名好友同樂成就，獲得『{1}』。"
		           ,"Congratulations! You have accomplished having fun with {0} friends fun. 『{1}』Get!");
        AddString (253, "每找{0}名朋友一起玩籃球黑幫可獲得國家隊球衣一件。",
		           "Invite {0} friends to join Gang of basketballers to get the National team jersey.");
		AddString (254, "第一次購買鑽石可獲得『{0}』。","Firstly, buying diamonds will receive a free『{0}』.");
		AddString (255, "未達開放等級8","Your team has not reaches Level 8.");
		AddString (256, "神奇徽章容量已達上限","Reaching the max \nMagical badge counts.");
		AddString (257, "[ffff00]{0}[-] {1}","[ffff00]{0}[-] {1}");

		AddString (259, "確認退出","Quit game?");
		AddString (260, "確定要退出遊戲嗎？","Quit game?");
		AddString (261, "玩家隊名還沒取喲","Please named your team.");
		AddString (262, "球隊資訊","Team Imfomation");
		AddString (263, "主場隊徽","Team Logo");
		AddString (264, "VIP資訊","VIP Imfomation");
		AddString (265, "開始時間","Start time");
		AddString (266, "結束時間","Finish time");
		AddString (268, "勳章","Badge");
		AddString (269, "音樂","Music");
		AddString (270, "音效","Sound");
		AddString (271, "問題回報","Report Bugs");
		AddString (272, "關卡取得兩顆星或是包月玩家即可使用 AI 喲！",
		           "Gain 2 stars in the levels or subscript monthly to use AI !");
		AddString (273, "投三分","Shoot a 3 point shot");
		AddString (274, "投兩分","Shoot a 2 point shot");
		AddString (275, "灌籃","Dunk"); //using in tutorial
		AddString (276, "防守控球者","Defense");
		AddString (277, "主動抄球","Steal");
		AddString (278, "蓋火鍋","Block"); //using in tutorial
		AddString (279, "尚未擁有該徽章","You don't have this Logo.");
		AddString (280, "本日徽章已領取，明天登入還可獲得新徽章喔～"
		           ,"You had already received today's badge gift, please come back tomorrow!");
		AddString (281, "教學中不能使用 AI 。","Can't use AI in tutorial stage.");
		AddString (282, "裝備已達強化上限","Reaching the max level.");
		AddString (283, "繼續比賽","Continue");
		AddString (284, "繼續上一場進行中比賽？\n第{0}關『{1}』","Continue the interrupted game? \n stage {0}  『{1}』");
		AddString (285, "包月成功！","You already join Monthly VIP.");
		AddString (286, "恭喜獲得800鑽！","Congratulations, get 800 diamonds.");
		AddString (287, "每日上線再領100鑽！","Get more 100 diamonds every day.");
		AddString (288, "技能","Skill");
        AddString (289, "▲點擊參賽模式：每日5次[99ff00](隨\nVIP等級增加)[-] / 使用[99ff00]參賽券[-]無限挑戰。\n▲選擇出賽球員。\n▲時限內快速[99ff00]累積灌籃距離[-]、灌爆籃筐！\n"
		           ,"▲ Select Entry Mode: 5 times [99ff00]a day(increased with \nVIP level) [-] / use [99ff00]entry ticket[-] Infinity Challenge. \n ▲ Select players. \n ▲  fast[99ff00] cumulative dunk distance within the time limit [-], Slam Dunk the basket! \n");
        AddString (290, "▲點擊參賽模式：每日5次[99ff00](隨\nVIP等級增加)[-] / 使用[99ff00]參賽券[-]無限挑戰。\n▲選擇出賽球員。\n▲時限內取得最高得分！範圍說明：\n[cc9933]深橘色[-] 0分、[ff6600]橘色[-] 50% 5分、\n[ffff00]黃色[-] 100% 7分、[ff0000]紅色[-] 100% 10分。\n▲[99ff00]3分球數值[-]越高的球員，橘、黃、紅色的[99ff00]範圍越寬[-]！"
		           ,"▲ Select Entry Mode: 5 times [99ff00]a day(increased with VIP level) [-] / use[99ff00] entry ticket[-] Infinity Challenge. \n▲ Select players to take part. \n▲Achieve the highest score within the time limit! Range Description: \n[cc9933]Dark orange[-] 0, [ff6600]Orange[-]50% 5points,\n [ffff00]Yellow[-] 100% 7points, [ff0000]Red[-] 100% 10points. \n▲ [99ff00]3-point-shot value[-] The taller the player is, the wider range of orange, yellow, and red's is!");//290
		AddString (291, "可挑戰", "Challenge");
		AddString (292, "您當前不能參加挑戰","Your team has not reach the require qualifications.");
		AddString (293, "尚未選滿挑戰隊伍","You haven't select a full challenge team.");
		AddString (294, "無參賽券","Your team has no Contest Tickets.");
		AddString (295, "球隊等級達到{0}級開放此功能。","Open this system when your team LV reach {0}.");
		AddString (296, "本次總灌籃距離：","Total distance:");
		AddString (297, "本次三分球得分：","Total points:");
		AddString (298, "球隊尚未取得對戰券","Your team has no PvP Tickets.");
		AddString (299, "{0}/{1}次","{0}/{1}");
		AddString (300, "很久之前","Long time ago");
		AddString (301, "{0}天前","{0}days ago");
		AddString (302, "發動技能{0}次","Use skill {0}");
		AddString (303, "無挑戰次數","You have no challenge times.");
		AddString (304, "無次數","You have no challenge times.");
		AddString (305, "找不到目標","No target.");
		AddString (307, "下一關 [99ff00]{0}[-] 開啓","Next stage [99ff00]{0}[-] Open");
		AddString (308, "恭喜通過！獲得","Congratulations to pass the stage. Get ");
		AddString (309, "挑戰失敗！","You failed.");
		AddString (310, "訓練球員，強化裝備都可以讓自己的球隊變強，更容易通過挑戰喲～","You can strengthen your team by training players or upgrade items.");
		AddString (311, "尚未挑戰成功","You haven't pass this stage.");
		AddString (312, "已領取獎勵","Got awards already.");
		AddString (313, "球隊資金","Team funds");
		AddString (314, "球隊等級到達２６才能參加例行賽。","Your team has to reaches 26 to attend the regular season.");
		AddString (315, "球隊須先選擇分區才能參加例行賽。","Your team has to select the division first.");
		AddString (316, "技能提升","Upgrade Skill");
		AddString (317, "主技能","Active Skill");
		AddString (318, "主動","Active");
		AddString (319, "被動","Passive");
		AddString (320, "挑戰黑幫魔王成功，請重置關卡繼續挑戰！","Challenge Gang Boss stages successful. Please reset it to challenge again.");
		AddString (321, "球隊技能點數不足。前往黑幫魔王關卡，賺取技能點數。","Skill points not enough.");
		AddString (322, "等級已達上限","Reach maximum level");
		AddString (323, "技能點數", "Skill Point");
		AddString (324, "Lv{0}/20", "Lv{0}/20");
		AddString (325, "Lv{0}", "Lv{0}");
		AddString (326, "AI啟動，再按一下按鈕可停止AI。","Turn on AI.");
		AddString (327, "籃球黑幫例行賽即將開打，敬請期待。");
		AddString (328, "網路傳輸異常，請在網路暢通的地方再試一次。","Your internet connection is not good. \nPlease try again later.");
		AddString (329, "AI關閉，再按一下按鈕可啓動AI。","Turn off AI.");
		AddString (330, "技能 Lv10 開啟潛能","Skill LV10 Open");
        AddString (331, "技能潛能功能尚未開放");
		AddString (332, "4星技", "2nd.Skill ");
		AddString (333, "練習賽","Reguler Season");
		AddString (334, "確定與『{0}』來場即時對戰練習賽？","Do you want play a real-time game versus『{0}』?");
		AddString (335, "Lv{0}/{1}","Lv{0}/{1}");
		AddString (336, "例行賽出了點狀況，請關閉介面再重試一次。","The game occur some problem,Please try again.");
		AddString (337, "目前不能挑戰此玩家，請選擇其他玩家進行挑戰。"
		           ,"Currently can't challenge this player;\n Please choose other players to challenge.");
		AddString (338, "發生了一點小問題，請離開比賽再重試一次。","Please try it again.");
		AddString (339, "鑽石","Diamond");
		AddString (340, "({0}/{1})","({0}/{1})");
        AddString (341, "加速啟動，再按一下按鈕可停止加速。");//
        AddString (342, "加速關閉，再按一下按鈕可啓動加速。");//
		AddString (343, "關卡達到三顆星，獲得獎勵鑽石{0}顆","You obtain 3 stage stars, get {0} Diamond.");
		AddString (344, "請先替球隊命名再參加例行賽。","Please entering your team name first.");
		AddString (345, "{0}例行賽積分:{1}","{0}Regular season points:{1}");
		AddString (346, "進入例行賽等待區，隨時可能遭受其他玩家挑戰。"
		           ,"You have entered the waiting area of the regular season. \nThe game will start automatically when you received a challenge.");
		AddString (347, "您已經離開例行賽等待區。","You already leave the waiting zone.");
		AddString (348, "積分還沒達到領獎標準，請再打個幾場吧。","Regular season points is not enough to get award.");
		AddString (349, "例行賽積分 + {0}","Regular season points + {0}");
		AddString (350, "『籃球黑幫-例行賽』活動開始，請點例行賽按鈕參加。"
		           ,"『Regular Season Game』is opening now,\nclick the『Regular season』button to join that.");
        AddString (351, "『籃球黑幫-例行賽』活動結束，感謝您的參與，明天同一時間準時開打。"
		           ," 『Regular-Season』is ended now.\n Thank you for your participation,\n play tomorrow at the same time on time!");
        AddString (352, "例行賽活動期間請點參賽按鈕，由系統配對對手。"
		           ,"Click the button during the regular season competition.\n The system will match opponents for you.");
        AddString (353, "例行賽活動時間為每天20:00~20:30，非活動時間您可與朋友自由切磋。"
		           ,"Regular-season is held at 20:00 to 20:30 everyday!\nDuring the other time you can compete with your friends freely.");
		AddString (354, "籃筐等級 {0}","Basket Lv {0}");
		AddString (356, "連線中斷，退出例行賽等待區。","A link may be disconnected, leaving the waiting zone."); 
		AddString (357, "比賽中不能兌換獎勵。","Please exchange award after game period."); 
		AddString (358, "更改隊名","Change your team name.");
        AddString (359, "更改隊名需要花費100顆鑽石，是否確定更換？");
		AddString (360, "球隊籃筐等級上升至{0}級。","Basket's Level upgrade to{0}");
		AddString (361, "技能 Lv 10 開啟潛能","Skill Lv10 Unlock"); 
		AddString (362, "{0}星覺醒後取得","{0}Star Obtain"); 
		AddString (363, "使用時機:{0}", "Using time:{0}"); 
		AddString (364, "發動時機:{0}", "Launch time:{0}"); 
		AddString (365, "發動率:{0}", "Launch Rate:{0}"); 
		AddString (366, "使用技能","Use skill"); 
		AddString (367, "發動4星技","Use 2nd.Skill"); 
		AddString (368, "發動5星技","Use 3rd.Skill"); 
		AddString (369, "尚未選滿鎮守人員","Players not enough.");
		AddString (370, "[ff0000]{0}[-] 後結束防守","After[ff0000]{0}[-] finished defence");
		AddString (371, "防守時間結束","Finished defence.");
		AddString (372, "{0} /小時", "{0} /H");
		AddString (373, "尚未佔領任何球場","You haven't occupied any court.");
		AddString (374, "對手無防守球隊，直接獲勝！","The opponent has no defense staff Win it hands down!"); 
		AddString (375, "{0}被{1}擊中，重傷退出比賽了！","{0}was hit by{1},he is retired from the contest with serious injuries ."); 
		AddString (376, "{0}被{1}甩出場外，退出這場比賽了！","{0} was thrown out of court \nthe by {1} and withdrew from the game!");
		AddString (378, "{0}成長筆記X{1}","{0}noteX{1}");
		AddString (379, "獲得『{0}』X1，累積足夠數量可邀請球員加入。","Gain『{0}』X1, you can got new player when you collected require amount。");
		AddString (380, "獲得『{0}』成長筆記X1，累積足夠數量可邀請球員加入。","Gain『{0}』X1, you can got new player when you collected require amount。");
		AddString (381, "尚未加入球隊","Not yet joined the team."); 
		AddString (382, "{0}成長筆記","{0}notes");
		AddString (383, "VIP等級需達到5,才能進行VIP選秀會。", "You have to reach VIP Level 5 to join the VIP Draft。");
		AddString (384, "您已累積購買了{0}鑽石。","Has bought  {0} diamonds so far.");
		AddString (385, "{0}擊爆籃球，比賽結束！","{0}smashed the ball，thw game was over！"); 
		AddString (386, "掃蕩", "Mop up");
        AddString (387, "重置", "Reset");
		AddString (388, "教學練習關卡無法掃蕩","You can't mop up Tutorial stages.");
		AddString (389, "無限","Unlimited");
		AddString (390, "購買關卡掃蕩券，快速通關！","Buy mop up tickets to pass the stage quickly.");
		AddString (391, "關卡編號錯誤","Stagedata Error");
		AddString (392, "今日挑戰次數已用完","After used all times of Stage Challenge."); 
		AddString (393, "今日買精力次數已用完","After used all times of buying energy.");
		AddString (394, "VIP等級不足","VIP Level is not enough.");
		AddString (395, "關卡尚未通過，無法掃蕩。","You haven't pass the stage.");
		AddString (396, "挑戰", "Challenge");
		AddString (397, "今日重置次數已用完","After used all times of reset.");
		AddString (398, "{0}","{0}");
		AddString (399, "再購買[00ffff]{0}[-]鑽石晉升為VIP","Buy more [00ffff]{0}[-]diamond to Promotion VIP");
		AddString (400, "天梯挑戰次數用完","After used all times of PvP Challenge.");
		AddString (401, "對手目前沒佔領球館","Opponent is not occuping the Arena  currently .");
		AddString (402, "已經被其他玩家佔領","It has been occupied by other players.");
		AddString (403, "『{0}』技能可以發動","『{0}』can be used");
		AddString (404, "球隊建立中...","Create team..."); 
		AddString (405, "比賽初始中...","Initial game..."); 
		AddString (406, "{0}/{1}\n積分排行", "{0}/{1}\nRank"); 
		AddString (407, "切換技能潛能","Change Potential");
		AddString (408, "消耗1點技能點?","Cost 1 skill point?");
		AddString (409, "徽章不足。","Badge not enough."); 
		AddString (410, "星等不足","Star LV not enough."); 
		AddString (411, "隨機獲得一項物品","Randomly obtain a item."); 
		AddString (412, "獲得所有物品","Obtain all items."); 
		AddString (413, "這麼好的獎勵一輩子只能領一次！","You could only get it once."); 
		AddString (414, "收集章節所有星星才能領取獎勵。","Collect all the stars in this charpter to receive rewards!"); 
		AddString (415, "需要另一個同等級物品。","Need the another same level item."); 
		AddString (416, "工程點數不足","Engineering Point");
		AddString (417, "請選擇同等級的相同裝備進行合成。","Drag the same item to combine.");
		AddString (418, "請選擇同等級的相同裝備進行合成。","Drag the same item to combine.");
		AddString (419, "已達上限等級","Reaching the max level");
		AddString (420, "裝備出處", "Source");

		AddString (1001, "裝備合成", "Combine Item");
		AddString (1002, "拖曳同等級同裝備進行合成", "Drag the same item to Combine.");
		AddString (1003, "原裝備", "Main");
		AddString (1004, "材料", "Material");
		AddString (1005, "合成後", "Result");
		AddString (1006, "需求金錢", "Spend money");
		AddString (1007, "確定合成", "Combine");
		AddString (1008, "參賽隊伍", "Participating team");
		AddString (1009, "等候系統配對中...", "Waiting for match...");
		AddString (1010, "獲得例行賽獎勵", "Got award");
		AddString (1011, "大賽說明", "Rules");
		AddString (1012, "[ff0000]真實對戰！你敢不敢？[-]", "[ff0000]Real-Time Game！[-]");
		AddString (1013, "◎[99ff00]活動[-]於每天[99ff00]20:00[-]準時開打！為時30分鐘，至[99ff00]20:30結束[-]。\n◎非活動時間也可進行對戰，但無法累積積分、兌換獎勵。\n◎在30分鐘內挑戰對手、累積積分，於活動結束時結算、\n[99ff00]當日24:00積分歸0[-]！\n◎進入戰局，不論輸贏皆能得到[99ff00]25分[-]獎勵！\n◎獲勝可得[99ff00]積分2倍[-]、   x2的獎勵；敗北也能獲得積分以及     x1的獎勵。\n◎獎勵等級分為五階，積分越高、獎勵越好！\n\n[fffea6]積分    50：    x50、      x50000、      x20\n積分  100：    x70、      x100000、    x50\n積分  200：    x100、    x150000、    x70\n積分  300：    x120、    x200000、    x100\n積分  500：    x150、    x300000、    x150[-]\n\n＊    [99ff00]工程點數[-]：玩家在熱身賽中擁有屬於自己的籃筐，\n使用[99ff00]工程點數升級你的籃筐[-]，提高對手挑戰你的分數，你也越難被打敗！最後，建議於Wifi環境下進行、保持連線暢通！"
		           ,"◎ Game start at [99ff00] 20:00 [-] everyday ! Last for 30 minutes, ends at [99ff00] 20:30 End[-].\n" +
		           "◎ Take the challenge within 30 minutes and earn points,reckon up at the end of the session, [99ff00]Points will be reset to zero at 24:00 daily[-] !\n" +
		           "◎ You can play in practive mode other than the sessions, but no rewards will be gained. \n" +
		           "◎ Enter the game and get [99ff00] 25 point [-] reward  regardless of winning or losing !\n" +
		           "◎ Winner can get [99ff00]double points[-],       x2 rewards; \n\nLoser can also gain points and       x1reward.\n" +
		           "◎ There are 5 reward levels, the higher the points are, the better the rewards are!" +
		           "\n\n  [fffea6]Points    50：    x50、      x50000、      x20\n  points  100：    x70、      x100000、    x50\n  points  200：    x100、    x150000、    x70\n  points  300：    x120、    x200000、    x100\n  points  500：    x150、    x300000、    x150[-]\n\n" +
		           "＊    [99ff00]Construction Points [-]: Players in the warm-up match have their own basket, \nuse the [99ff00]Construction points to upgrade your basket[-] to increase the \nopponents's score to challenge you ,and make yourseld more difficult to be defeated! \n\n\n Finally, it is recommended to play with Wifi to maintain smooth connection!");
		AddString (1014, "點擊可查看裝備資訊", "Click for check information.");
		AddString (1015, "售出", "Sell");
		AddString (1016, "獲得途徑", "Source");
		AddString (1017, "請先放置原裝備", "Please set source Items first.");
		AddString (1018, "請放置相同裝備", "Please set the same Items.");
		AddString (1019, "選擇球隊徽章", "Select Team Logo");

		AddString (20001, "無此關卡","No stages.");
		AddString (20002, "第{0}章", "Ch. {0}");
		AddString (20003, "無能量飲，前往表演賽取得額外精力！","You have no energy drink, go Exhibition Match to get it.");
		AddString (20004, "黑幫魔王第{0}戰","Gang Boss {0}Class");
		AddString (20005, "[ff6600]{0}[-]","[ff6600]{0}[-]");
		AddString (20007, "資金","Funds");
		AddString (20008, "商品","Goods");
		AddString (20009, "購買商品","Purchase goods");
		AddString (20010, "購買{0}X{1}。","Purchase{0}\nX{1}。");
		AddString (20011, "獲得『{0}』X{1}","Get『{0}』\nX{1}");
		AddString (20012, "有非法字元","Illegal words.");
		AddString (20013, "設定成功","Setup Completed");
		AddString (20014, "不可設定同一句話","You can't enter same words.");
		AddString (20015, "頭飾", "Headband");
		AddString (20016, "球衣", "Jersey");
		AddString (20017, "鞋子", "Shoes");
		AddString (20018, "飾品", "Accessories");
		AddString (20019, "袖套", "Elbow Wrap");
		AddString (20020, "褲襪", "Leggings");
		AddString (20021, "2分", "2-point");
		AddString (20022, "3分", "3-point");
		AddString (20023, "灌籃", "Dunk");
		AddString (20024, "速度", "Speed");
		AddString (20025, "控球", "Dribble");
		AddString (20026, "抄截", "Steal");
		AddString (20027, "火鍋", "Block");
		AddString (20028, "干擾","Interference");
		AddString (20029, "閃避", "Dodge");
		AddString (20030, "霸氣","Domineering");
		AddString (20031, "減速度","Reducing Speed");
		AddString (20032, "：", "：");
		AddString (20033, "我方進攻","Offensive");
		AddString (20034, "我方防守","Defensive");
		AddString (20035, "無限制","Limitless");
		AddString (20036, "進球後","After scoring");
		AddString (20037, "傳球時","Passing");
		AddString (20038, "投籃時","Shooting");
		AddString (20039, "灌籃時","Dunking");
		AddString (20040, "抄截時","Stealing");
		AddString (20041, "火鍋時","Blocking");
		AddString (20042, "搶籃板時","Rebounding");
		AddString (20043, "被抄截時","Be Stolen");
		AddString (20044, "被火鍋時","Be Blocked");
		AddString (20045, "靠近籃筐","Near the basket");
		AddString (20046, "PG", "PG");
		AddString (20047, "SG", "SG");
		AddString (20048, "SF", "SF");
		AddString (20049, "PF", "PF");
		AddString (20050, "C", "C");
		AddString (20051, "一","I");
		AddString (20052, "二","II");
		AddString (20053, "三","III");
		AddString (20054, "四","IV");
		AddString (20055, "五","V");
		AddString (20056, "六","VI");
		AddString (20057, "七","VII");
		AddString (20058, "八","VIII");
		AddString (20059, "九","IX");
		AddString (20060, "十","X");
		AddString (20061, "關卡", "Stages");
		AddString (20062, "表演賽", "Exhibition Match");
		AddString (20063, "黑幫魔王","Gang Boss");
		AddString (20064, "天梯", "PvP");
		AddString (20065, "例行賽","Regular Season");
		AddString (20066, "任務", "Quests");
		AddString (20067, "天梯排行", "PvP Rankings");
		AddString (20068, "裝備","Equipment");
		AddString (20069, "選秀", "Draft");
		AddString (20070, "黑幫魔王","Gang Boss");
		AddString (20071, "第{0}戰"," {0} Class");
		AddString (20072, "目前進度","Current progress");
		AddString (20073, "規則", "Rules");
		AddString (20074, "Lv{0} 開啓","Lv{0} Open");
		AddString (20075, "黑幫魔王三連關！\n\n打敗[99ff00]最終魔王[-]－\n就能得到豐厚獎勵！"
		           ,"Gang Boss triple!\nbeat the \n[99ff00]ultimate Boss[-] -\nto get rich rewards!");
		AddString (20076, "可能獲得","Probably get");
		AddString (20077, "剩餘次數","Remaining Times");
		AddString (20078, "領取獎勵", "Award");
		AddString (20079, "恭喜獲得","Congratulation to obtain");
		AddString (20080, "是否確定[99ff00]重置[-]黑幫魔王？","Be sure [99ff00]reset[-] Gang Boss stages？");
		AddString (20081, "注意：未領取的獎勵將會消失。","Notice: The Uncollected awards will disappear.");
		AddString (20082, "一群穿著道服、自稱魔王的怪傢伙，四處挑戰球隊！\n他們使用奇怪的[99ff00]格鬥技巧[-]打敗群雄！\n\n而你，敢不敢挑戰他們！？","A bunch of strange dudes wearing robes and call themselves Boss is challenging teams everywhere!  \n They use strange [99ff00] fighting skills [-] to play basketball! \n \n Do you dare to challenge them!?");
		AddString (20083, "1.球隊15級開啟第一戰，每隔10級開放下一戰。\n2. 每戰有三局球賽，贏得[99ff00]勝利後可領取豐厚獎勵[-]。\n3. 每局皆[99ff00]可挑戰5次[-]，打輸了可以重複挑戰，直到次數用完。\n4. 可進行的每一戰都挑戰完後，每天可以[99ff00]重置1次[-]黑幫魔王(隨VIP等級增加)。\n5. [ffffa0]擊敗黑幫魔王與他的小夥伴們，就能獲得珍貴的[ff6464]技能點數[-]、強化球隊！","1. The team will open the first battle on Lv 15, and open next battle every 10 level. \n2. In every battle there are three games. The winning rewards are [99ff00] skill points as well as Boss's growth notes! [-]. \n3. Therer are [99ff00] 5 chances per game [-], make sure you seize them! . \n4. You can [99ff00] resets once a day [-] Gang Boss  ( increases with VIP level).");
		AddString (20084, "灌籃大賽","Slam Dunk Contest");
		AddString (20085, "三分球大賽","3-point Shootout");
		AddString (20086, "目前排名","Current Rank");
		AddString (20087, "最佳成績","Score");
		AddString (20088, "一鍵配置球員","Auto setting");
		AddString (20089, "上場！","Go!");
		AddString (20090, "兌換","Exchange");
		AddString (20091, "▲點擊參賽模式(每日5次[99ff00](隨\nVIP等級增加)[-] / 使用[99ff00]參賽券[-]無限挑戰)。\n▲選擇出賽球員。\n▲時限內快速[99ff00]累積灌籃距離[-]、灌爆籃筐！\n▲每次比賽都能得到2枚[99ff00]大賽徽章[-]"
					,"▲ Select entry mode (5 times a day [99ff00] (incresed with \nVIP grade increase) [-] / use [99ff00] entry ticket [-] Infinity Challenge). \n ▲Select players. \n ▲  fast [99ff00] cumulative dunk distance within the time limit [-], Slum Dunk the basket! \n▲ Each game, you can get two [99ff00] contest badge [-].");
		AddString (20092, "我的最佳成績","My best score");
		AddString (20093, "排行榜","Ranking");
		AddString (20094, "今日次數","Times");
		AddString (20095, "等待時間","Waiting time");
		AddString (20096, "無次數上限、等待時間！","No maximum times.\nNo waiting time!");
		AddString (20097, "參賽券","Exhibition Match tickets");
		AddString (20098, "參賽", "Go!");
		AddString (20099, "使用參賽券","Use Tickets");
		AddString (20100, "每日活動", "Daily Events");
		AddString (20101, "神奇徽章", "Magical Logo");
		AddString (20102, "大西洋區", "Atlantic");
		AddString (20103, "西北區", "Northwest");	
		AddString (20104, "中央區", "Central");
		AddString (20105, "東南區", "Southeast");
		AddString (20106, "太平洋區", "Pacific");
		AddString (20107, "西南區", "Southwest");
		AddString (20108, "徽章上限", "Reaching the max Logo counts");
		AddString (20109, "點擊鎖頭、\n解開上限！", "Click Locks、\n Unlock Limit!");
		AddString (20110, "徽章禮盒", "Magical Logo\nGift");
		AddString (20111, "每日上線都能領取，隨機獲得神奇徽章並為球隊增加數值！\n達成橫向連線還有額外獎勵！"
					,"Receive by logged in daily and get magical badge with attibutes ramdonly!\n Accomplish the widthways connections to get extra bonus!");
		AddString (20112, "開啟", "Open");
		AddString (20113, "獲得徽章", "Get Magical Logo");
		AddString (20114, "頂級球星", "Top Players");
		AddString (20115, "頂級球隊", "Top Teams");
		AddString (20116, "好友", "Friends");
		AddString (20117, "天梯數據成績", "PvP Stats");
		AddString (20118, "*每週更新*", "*Updated weekly*");
		AddString (20119, "得分", "Score");
		AddString (20120, "籃板", "Rebound");
		AddString (20121, "火鍋", "Block");
		AddString (20122, "抄截", "Steal");
		AddString (20123, "助攻", "Assists");
		AddString (20124, "連結臉書帳號與好友同樂就送[99ff00]『非洲大山·穆湯波』[-]，搭配超強絕招[99ff00]『麻辣火鍋』[-]，讓你海電對手易如反掌。"
					,"Link FB account abd play with the with friends to get [99ff00] 『African mountains · Mutombo』 [-],use with the super trick [99ff00]『Spicy hotpot』 [-], to beat your opponent like a pice of cake!");
		AddString (20125, "*FB連結將歷時30~60秒，期間請不要關閉遊戲。","Connecting to Facebook. Please don't quit game.");
		AddString (20126, "*若你的FB帳號曾連結其他裝置中的籃球黑幫，將以其他裝置上的遊戲記錄與儲值記錄\n覆蓋掉此裝置上的，[99ff00]此裝置記錄則會消失[-]，請確認後再執行。"
					,"Connecting to Facebook. If your FB account has connected to Gang of basketballers on other devices, the game and purchase recorde on the previous device will repalce these ones.[99ff00] Records on this device will disappear [-], please confirm before subbmit");
		AddString (20127, "街頭球場", "Street Court");
		AddString (20128, "VIP球場", "VIP Court");
		AddString (20129, "防守結果", "Defence Results");
		AddString (20130, "規則", "Rules");
		AddString (20131, "對手戰力", "Opponent's Force");
		AddString (20132, "佔領街頭球場，","Occupied the street court");
		AddString (20133, "贏取鑽石與技能點數！","and get diamond and skill points.");
		AddString (20134, "搜尋球場","Search");
		AddString (20135, "防守時間：","Occupied Time :");
		AddString (20136, "防守獎勵：","Occupied Awards :");
		AddString (20137, "[99ff00]10[-] 小時", "[99ff00]10[-] Hours");
		AddString (20138, "勝利","Finish");
		AddString (20139, "成功！","Success !");
		AddString (20140, "領取","Get");
		AddString (20141, "失敗","Fail");
		AddString (20142, "挑戰你，你輸了。","You lose !");

		AddString (20143, "◎20、33級開啓街頭球場系統。\n◎花費球隊資金進行球場搜尋，擊敗看守球場者並佔領該球\n場。\n◎球場佔領[99ff00]為時10小時[-]，期間有機會遭到其他玩家挑戰並奪\n走球場。\n◎遭到其他玩家挑戰成功會失去一部份的佔領獎勵，獎勵訊\n息在[99ff00]防守結果[-]頁面。\n◎挑戰其他玩家並成功佔領，也能奪走對方一部份獎勵。\n◎請記得在[99ff00]佔領球場後，派出球員在球場裡駐守[-]。\n◎佔領球場能夠獲得球隊資金、技能點數以及鑽石的獎勵。\n◎[99ff00]VIP等級2[-]能夠開啟第3個球場佇列。\n\n\n最後，勤加練習！" 
		           ,"◎ Lv20, Lv33 open court system.\n◎ Spent team fund searching for courts, beat the court guards and occupied the court.\n◎ Court occupation last for [99ff00]10 hours[-]. It has the opportunity to be challenged by other players and take over the court.\n◎ If successfully challenged by other players, you will lose a part of the occupation rewards. Reward messages are on the [99ff00]Defense Results[-] page.\n◎ Challenge other players and occupied successfully can also take away a part of their awards. \n◎ Please remember [99ff00]After the occupied the court, sent a player to guard it[-].\n◎ Occupation rewards are team funds, skill points and aa few diamonds.\n◎ [99ff00]VIP Level 2[-] is able to open the third court.\n\n\n Last but not least, PRACTICE MORE!");

		AddString (20144, "球員覺醒","Upgrade");
		AddString (20145, "獲得途徑","Source");
		AddString (20146, "對戰成績","Stats");
		AddString (20147, "覺醒需求","Upgrade cost");
		AddString (20148, "點擊選擇球員","Touch screen to choose player.");
		AddString (20149, "選擇球員","Choose players");
		AddString (20150, "點擊頭像更換球員","Touch player pic to change.");
		AddString (20151, "開始訓練","Start training");
		AddString (20152, "消除時間","Finish Now");
		AddString (20153, "達成條件自動開啓","Achieving goals to unlock.");
		AddString (20154, "直接開啓","Unlock");
		AddString (20155, "訓練完成","Complete training");
		AddString (20156, "訓練中","Training");
		AddString (20157, "說明","Explanation");
		AddString (20158, "儲值馬上送800鑽石","Get extra 800 diamond");
		AddString (20159, "每天登入、再送100Gem","Get 100 Diamond Eveyday");
		AddString (20160, "關卡AI功能開啟！","Unlock stage AI button.");
		AddString (20161, "包月VIP","Monthly VIP");
		AddString (20162, "一大箱","Big box");
		AddString (20163, "再送！","Bonus!");
		AddString (20164, "(限購1次)","(You can only\npurchase once)");
		AddString (20165, "& 超級英雄球星福袋","& Super Hero gift set");
		AddString (20166, "一小箱","Small box");
		AddString (20167, "一麻袋","Big bag");
		AddString (20168, "一小袋","Small bag");
		AddString (20169, "一小堆","A pile");
		AddString (20170, "& 公牛王朝球星福袋","& Bulls Dynasty gift set");
		AddString (20171, "球隊資金{0}萬","Team funds {0}0000");
		AddString (20172, "VIP{0}特權","VIP{0} special");
		AddString (20173, "1.每日主線關卡重置次數 +1\n2.每日獲得 5 張掃蕩券\n3.每日可購買精力次數 +1"
					,"Daily \n1.Stage Reset +1\n2.Get 5 mop up tickets \n3.Buy energy +1 ");
		AddString (20174, "1.可佔領的街頭球場 + 1\n2.每日獲得 10 張掃蕩券  \n3.每日可獲得 1 個神奇徽章寶箱\n4.每日黑幫魔王重置次數 +1\n5.每日灌籃賽次數 +1\n6.每日三分賽次數 +1\n7.每日天梯挑戰次數 +1\n8.每日可購買精力次數 +2"
					,"Daily \n1.Street court +1\n2.Get 10 mop up tickets\n3.Get 1 magical logo set\n4.Gang boss stage reset +1\n5.Slam Dunk Contest +1\n6.Three-Point Shootout+1\n7.PvP Challenge+1\n8.Buy energy +2");
		AddString (20175, "1.每日獲得 20 張掃蕩券\n2.每日主線關卡重置次數 +2\n3.每日可獲得 2 個神奇徽章寶箱\n4.每日可購買精力次數 +3"
					,"Daily \n1.Get 20 mop up tickets\n2.Stage Reset +2\n3.Get 2 magical logo set\n4.Buy energy +3");
		AddString (20176, "1.每日獲得 40 張掃蕩券\n2.每日黑幫魔王重置次數 +2\n3.每日灌籃賽次數 +2\n4.每日三分賽次數 +2\n5.每日天梯挑戰次數 +2\n6.每日可購買精力次數+4"
					,"Daily \n1.Get 40 mop up tickets\n2.Gang boss stage reset +2\n3.Slam Dunk Contest +2\n4.Three-Point Shootout+2\n5.PvP Challenge+2\n6.Buy energy +4");
		AddString (20177, "1.獲得參與頂級球星選秀的資格\n2.每日主線關卡重置次數 +3\n3.每日可獲得 3 個神奇徽章寶箱 \n4.每日可購買精力次數 +5"
					,"Daily \n1.Unlock top players draft\n2.Stage Reset +3\n3.Get 3 magical logo set\n4.Buy energy +5");
		AddString (20178, "1.技能升級金額 -50%\n2.每日獲得 60 張掃蕩券\n3.每日黑幫魔王重置次數 +3\n4.每日灌籃賽次數 +3\n5.每日三分賽次數 +3\n6.每日天梯挑戰次數 +3\n7.每日可購買精力次數 +6"
					,"Daily \n1.Upgrade skill spent money -50% +1\n2.Get 60 mop up tickets\n3.Gang boss stage reset +3\n4.Slam Dunk Contest +3\n5.Three-Point Shootout+3\n6.PvP Challenge+3\n7.Buy energy +6");
		AddString (20179, "1.主線關卡獲得金錢 2 倍\n2.每日主線關卡重置次數+4\n3.每日可獲得 4 個神奇徽章寶箱 \n4.每日可購買精力次數 +7"
					,"Daily \n1.Stage monet award x2 +1\n2.Stage Reset +4\n3.Get 4 magical logo set\n4.Buy energy +7");
		AddString (20180, "1.每日獲得 80 張掃蕩券\n2.每日黑幫魔王重置次數 +4\n3.每日灌籃賽次數 +4\n4.每日三分賽次數 +4\n5.每日天梯挑戰次數 +4\n6.每日可購買精力次數 +8"
					,"Daily \n1.Get 80 mop up tickets\n2.Gang boss stage reset +4\n3.Slam Dunk Contest +4\n4.Three-Point Shootout+4\n5.PvP Challenge+4\n6.Buy energy +8");
		AddString (20181, "先發五人\n總戰力","Starting Lineup\nForce");
		AddString (20182, "系統設定","Options");
		AddString (20183, "自訂\n問候語","Greetings");
		AddString (20184, "特效","Effect");
		AddString (20185, "前往\n專頁","Link\nFans Page");
		AddString (20186, "語言","Language");
		AddString (20187, "技能提示","Skill\nExplanation");
		AddString (20188, "遊戲公告","What's New");
		AddString (20189, "更新資訊","Updata Info");
		AddString (20190, "*慢動作提示開關，\n開啟後在技能可施放狀況下會出現慢動作提示。"
		           ,"* Slow motion sign switch, \n When switche on, the slow motion  sign will be showed while the skills can be cast. ");
		AddString (20191, "點選頭圖更換上場球員。","Click the player pic to change the starter.");
		AddString (20192, "累積灌籃距離：","Total dunk distance:");
		AddString (20193, "三分球累積分數：","Total 3-points scores:");
		AddString (20194, "灌籃距離", "Distance");
		AddString (20195, "挑戰/掃蕩：","Challenge/Mop up:");
		AddString (20196, "掃蕩券","Mop up tickets");
		AddString (20197, "參賽隊伍","Competing Teams");
		AddString (20198, "大賽說明","Game rules");
		AddString (20199, "[ff0000]真實對戰！你敢不敢？[-]","[ff0000]Real-time Game！[-]");
		AddString (20200, "◎[99ff00]活動[-]於每天[99ff00]20:00[-]準時開打！為時30分鐘，至[99ff00]20:30結束[-]。\n" +
					"◎非活動時間也可進行對戰，但無法累積積分、兌換獎勵。\n" +
					"◎在30分鐘內挑戰對手、累積積分，於活動結束時結算、\n  [99ff00]當日24:00積分歸0[-]！\n" +
					"◎進入戰局，不論輸贏皆能得到[99ff00]25分[-]獎勵！\n" +
					"◎獲勝可得[99ff00]積分2倍[-]、   x2的獎勵；敗北也能獲得積分以及     x1\n  的獎勵。\n" +
					"◎獎勵等級分為五階，積分越高、獎勵越好！\n  [fffea6]積分    50：    x50、      x50000、      x20" +
					"\n  積分  100：    x70、      x100000、    x50\n  積分  200：    x100、    x150000、    x70" +
					"\n  積分  300：    x120、    x200000、    x100\n  積分  500：    x150、    x300000、    x150[-]" +
					"\n\n＊    [99ff00]工程點數[-]：玩家在熱身賽中擁有屬於自己的籃筐，\n  使用[99ff00]工程點數升級你的籃筐[-]，提高對手挑戰你的分數，\n  你也越難被打敗！\n\n最後，建議於Wifi環境下進行、保持連線暢通！"
		           ,"◎ Game start at [99ff00] 20:00 [-] everyday ! Last for 30 minutes, ends at [99ff00] 20:30 End [-].\n" +
		           "◎ Take the challenge within 30 minutes and earn points,reckon up at the end of the session, \n [99ff00] Points will be reset to zero at 24:00 daily [-]! \n " +
		           "◎ You can play in practive mode other than the sessions, but no rewards will be gained. \n" +
		           "◎ Enter the game and get [99ff00] 25 point [-] reward  regardless of winning or losing !\n" +
		           "◎ winner can get [99ff00] double points [-], x2 rewards; Loser can also gain points and x1 \n reward.\n" +
		           "◎ There are 5 reward levels, the higher the points are, the better the rewards are!" +
		           "\n  [fffea6]Points    50：    x50、      x50000、      x20\n  points  100：    x70、      x100000、    x50\n  points  200：    x100、    x150000、    x70\n  points  300：    x120、    x200000、    x100\n  points  500：    x150、    x300000、    x150[-]\n\n" +
		           "＊  [99ff00] Construction Points [-]: Players in the warm-up match have their own basket, \n use the [99ff00] Construction points to upgrade your basket [-] to increase the opponents's score to challenge you , \n and make yourseld more difficult to be defeated! \n\n Finally, it is recommended to play with  Wifi  to maintain smooth connection!");
		AddString (20201, "設定球隊名稱","Enter your team name.");
		AddString (20202, "球隊名稱(char 2-20)","Team name(char 2-20)");
		AddString (20203, "隨機取名","Random ");
		AddString (20204, "設定球隊問候語","Enter Greetings");
		AddString (20205, "球隊問候語(char 1-50)","Greetings (char 1-50)");
		AddString (20206, "將會顯示於天梯、黑幫魔王以及例行中，不設定則顯示系統預設。"
		           ,"Will be displayed on the stage of PvP,\nGang Boss and the Regular season. No default system settings.");
		AddString (20207, "   加入\n包月VIP！"," Join\n Monthly VIP！");
		AddString (20208, "鑽石不足","Diamond not enough.");
		AddString (20209, "前往\nGoogle Play商店","Get it on\nGoogle Play");
		AddString (20210, "前往\nDropBox下載頁面","Get it on\nDropbox");
		AddString (20211, "分區選擇","Select Division");
		AddString (20212, "選擇加入區域聯盟－開始[fff000]天梯挑戰[-]","Select Division to play[fff000]PvP Challenge[-]");
		AddString (20213, "更換裝備","Item Change");
		AddString (20214, "重試","Retry");
		AddString (20215, "選擇隊長位置","Select Captain's position");
		AddString (20216, "PG 控球後衛   魔術·強森","Point Guard - M.Johnsonn");
		AddString (20217, "SG 得分後衛   雷吉·米勒","Shooting Guard - R.Millerr");
		AddString (20218, "SF 小前鋒   賴瑞·博德","Small Forward - L.Birdd");
		AddString (20219, "PF 大前鋒 查爾斯·巴克利","Power Forward - C.Barkleyy");
		AddString (20220, "C 中鋒 派屈克·尤英","Center - P.Ewingg");
		AddString (20221, "上一步","Back");
		AddString (20222, "完成","Finish");
		AddString (20223, "創造球隊","Create Team");
		AddString (20224, "臉型","Face");
		AddString (20225, "隊徽","Magical LOGO");
		AddString (20226, "背號","Number");
		AddString (20227, "下一步","Next");
		AddString (20228, "一鍵練滿","Upgrade to\nMax Lv");
		AddString (20229, "需求", "Demand");
		AddString (20230, "登入中...","Login...");
		AddString (20231, "載入檔案中...","Loading...");
		AddString (20232, "下載檔案中...","Downloading...");
		AddString (20233, "最多屬性數", "Maximum Attribute");
		AddString (20234, "初階洗屬性","Basic attribute enhance");
		AddString (20235, "進階洗屬性","Advanced attribute enhance");
		AddString (20236, "已達最大數值","This item has already reached the maximum number of attribute.");
		AddString (20237, "此裝備無數值","This item has no attribute.");
		AddString (20238, "挑戰成功","Successful");//UIGame 挑戰成功
		AddString (20239, "挑戰失敗","Fail");//UIGame 挑戰失敗
		AddString (20240, "街頭球場","Street Court");
		AddString (20241, "選擇鎮守的球員","Choose who to guard");
		AddString (20242, "一鍵配置球員","Auto setting");
		AddString (20243, "防守開始", "YES");
		AddString (20244, "防守球場中⋯", "Guard....");
		AddString (20245, "VIP好康", "VIP Best Value");
		AddString (20246, "每天登入、再送100", "Get 100 Diamond Eveyday");
		AddString (20247, "連續\n30天！", "30 \nDays!");
		AddString (20248, "關卡AI功能開啟！", "Unlock stage AI button.");
		AddString (20249, "花費 NT180 獲得包月VIP！", "Spend $6 to get Monthly VIP!");
		AddString (20250, "灌籃失敗！", "DUNK FAIL!");
		AddString (20251, "大賽成績", "Your Score");
		AddString (20252, "獲得防守獎勵", "Got Rewards");
		AddString (20253, "得到剩餘獎勵", "Got the remaining rewards");
		AddString (20254, "([ff0000]{0}[-]/{1})", "([ff0000]{0}[-]/{1})");

		//knight40
		AddString (30001, "集滿獎勵", "Collected all awards");
		AddString (30003, "獲得關卡獎勵", "Got stage rewards");
		AddString (30004, "掃蕩券", "Mop up\nTickets");
		AddString (30005, "秒內過關", "S pass\nthis stage");
		AddString (30006, "失分小於", "Lose points\nunder ");
		AddString (30007, "技能絕殺", "Use skill to get \nGame-winner Shot");
		AddString (30008, "獎勵", "Awards");
		AddString (30009, "精力", "Energy");
		AddString (30010, "2星：開啟關卡AI權限。3星：獲得獎勵[99ff00]10[-]", "2Stars：Unlock AI。3Stars：Gain Award[99ff00]10[-]");
		AddString (30101, "技能潛能", "Skill Potential");
		AddString (30102, "升級後", "After upgrade");
		AddString (30103, "主技能 Lv10 可開啟潛能", "Skill Lv 10 Unlock");
		AddString (30104, "(由黑幫魔王獲得)", "(Soure:Gang Boss stage)");
		AddString (30105, "技能 Lv 20 開啟潛能", "Skill Lv 20 Unlock");
		AddString (30106, "被動技無潛能", "This skill NO Potential");
		AddString (30201, "國際霸權", "National Supremacy");
		AddString (30202, "國家菁英", "National Elite");
		AddString (30203, "職業強隊", "Heavyweight \nProfessional Team ");
		AddString (30204, "威震鄉里", "Majesty  \nAwed world-widely");
		AddString (30205, "小型球隊", "Small Team");
		AddString (30206, "對戰券", "PvP Challenge Tickets");
		AddString (30207, "使用對戰券", "Use Tickets");
		AddString (30208, "冷卻時間", "Cooldown");
		AddString (30209, "排名 / 球隊名稱", "Ranking /\nTeam name");
		AddString (30210, "獲勝獎勵", "Awards");
		AddString (30301, "球隊設施", "Team Facilities");
		AddString (30302, "工程點數", "Construction Points");
		AddString (30303, "我方籃筐分數", "Our target");
		AddString (30304, "[ffff00]升級的方式[-]\n▲參加每晚8:00的熱身賽活動，累積[99ff00]工程點\n數[-]。\n▲累積達[99ff00]可升級數量[-]後進行升級！\n[ffff00]籃筐的用途[-]\n▲[99ff00]籃筐等級+1，我方籃筐分數也+1！[-]籃筐等\n 級越高，你的對手就越難打敗你喲！", 
		           "[ffff00] How to upgrade [-] \n▲ Take part in the regular season at 8:00 every night, the cumulative [99ff00] engineering point[-]. \n "+
		           "▲ Accumulated to [99ff00] specific number and upgrade! \n[ffff00]The uses of the basket .[-] " +
		           "\n▲ [99ff00] When basket level +1, our basket level +1, too! [-]The higher level the basket is, the harder it is for your opponent to beat you! ");
		AddString (30305, "升級", "Level UP");
		AddString (30401, "選秀會", "Draft");
		AddString (30402, "頂級10連選！", "10 Draft！");
		AddString (30403, "《至少延攬一位球星》", "《At least recruit 1 star》");
		AddString (30404, "進行 1 次選秀！", "1 Draft");
		AddString (30405, "Free! 免費選", "Free Draft");
		AddString (30406, "距離下次\n還有", "CD time");
		AddString (30407, "VIP5\n選秀", "VIP5\nDraft");
		AddString (30408, "一般選秀", "Draft");
		AddString (30501, "星等", "Star Level");
		AddString (30502, "戰力", "Force");
		AddString (30503, "5星技", "3rd.Skill");
		AddString (30601, "恭喜獲得", "Congratulation");

		AddString (30701, "操作模式", "Control Mode");
		AddString (30702, "經典搖桿", "Joystick");
		AddString (30703, "手指觸碰", "Touch");
		AddString (30704, "競賽開始", "Start");
		AddString (30705, "競賽視角", "Game view");
		AddString (30706, "垂直視角", "Vertically");
		AddString (30707, "橫向視角", "Horizontal");
		AddString (30708, "＊遊戲會自動保存設定。", "＊ The game will save the settings automatically");
		AddString (30709, "發動技能", "Use Skill");
		AddString (30710, "離開比賽", "Quit");
		AddString (30711, "挑戰成功", "Success challenge");
		AddString (30712, "我方先得", "We have to get");
		AddString (30714, "敵方先得", "Opponent have to get");
		AddString (30715, "點擊球員圖像觀看情報", "Click player's pic to see their info.");
		AddString (30716, "結束比賽", "Quit Game");
		AddString (30717, "分", "scores");
		AddString (30718, "你贏得了勝利！得到", "You WIN!");
		AddString (30719, "獲得戰利品", "WIN awards");
		AddString (30801, "熱身賽積分", "Regular season points");
		AddString (30802, "時間獎勵", "Ontime Award");
		AddString (30803, "下一關", "Next");
		AddString (30804, "再玩一次", "Again");
		AddString (30805, "籃板", "Board");
		AddString (30806, "助攻", "Assists");
		AddString (30901, "章節解鎖", "Unlock New Chapter ");
		AddString (31001, "球隊等級:", "Team LV");
		AddString (31002, "精力上限:", "Max Energy");
		AddString (31003, "資金上限:", "Max Funds");
		AddString (31004, "開放功能：", "New Feature");
		AddString (31005, "球隊升級", "Level Up");
		
		AddString (40001, "幹");
		AddString (40002, "你娘");
		AddString (40003, "FUCK");
		AddString (40004, "fuck");
		AddString (40005, "雞掰");
		AddString (40006, "機掰");
		#if OutFile
		StringWrite("TW");
		#endif
    }

    public static string GetNumberText(int Value){
        string Result = "";
		string Temp = Value.ToString ();
		int TempValue;
		if (Value <= 10) {
			Result = S (20050 + Value);
		}else if(Value >= 11 && Value <= 19){
			for (int i = 0; i < Temp.Length; i++) {
				if(int.TryParse(Temp[i].ToString(), out TempValue)){
					if(i == 0)
						Result += S (20060);
					else
						Result += S (20050 + TempValue);
				}
			}
		}else if(Value % 10 == 0){
			for (int i = 0; i < Temp.Length; i++) {
				if(int.TryParse(Temp[i].ToString(), out TempValue)){
					if(i == 0)
						Result += S (20050 + TempValue);
					else
						Result += S (20060);
				}
			}
		}else{
			for (int i = 0; i < Temp.Length; i++) {
				if(int.TryParse(Temp[i].ToString(), out TempValue)){
					if(i == 0)
						Result += S (20050 + TempValue) + S (20060);
					else
						Result += S (20050 + TempValue);
				}
			}
		}	
		
		return Result;
	}
    
    public static string PositionEN(int index)
    {
		if(index >= 0 && index <= 4)
			return S(20046 + index);
        else
            return "";
    }

    public static string ItemPosition(int index)
    {
		if (index >= ParameterConst.Min_EquipmentKind && index <= ParameterConst.Max_EquipmentKind)
			return S(20014 + index);
		else
			return "";
    }
    
    public static string ItemEffectStr(int index, bool NoSymbol = false)
    {
		if (index >= ParameterConst.Min_ItemEffect && index <= ParameterConst.Max_ItemEffect) {
			if(NoSymbol)
				return S(20021 + index - ParameterConst.Min_ItemEffect);
			else
				return S(20021 + index - ParameterConst.Min_ItemEffect) + S(20032);
		}else
			return "";
    }

    public static string LogoStr(int index){
//        index -= 4;
//		if(index >= 0 && index < ParameterConst.Max_LogoBingo)
//			return TeamManager.DItems[14 + index].Name;
//        else{
//            Debug.Log("LogoStr:" + index.ToString());
            return "";
//        }
    }

    public static string SkillSituationText(int skillNo) {
//		if (TeamManager.DSkills.ContainsKey(skillNo) && TeamManager.DSkills[skillNo].Situation >= 1 && TeamManager.DSkills[skillNo].Situation <= 13)
//			return S(20032 + TeamManager.DSkills[skillNo].Situation);
//        else
            return "";
    }

	public static bool HaveDirtyWord(string Value){
		bool Result = false;

		if(Value.Length > 0){
			for(int i = 40001; i <= 40006; i++){
				if(Value.Contains(TextConst.S(i))){
					Result = true;
					break;
				}
			}
		}

		return Result;
	}

	#if OutFile
	private static void StringWrite(string fileName)
	{
		try
		{
			string Path = Application.dataPath + "/Resources/GameData/" + fileName +".txt";
			FileStream myFile = File.Open(Path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
			StreamWriter myWriter = new StreamWriter(myFile);
			myWriter.Write(sb.ToString());
			myWriter.Close();
			myFile.Close();
		}
		catch
		{
		} 
	}
	#endif
}
