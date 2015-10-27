using UnityEngine;
using System.Collections;
using Knight49.Converter;

public class KnightZhConverter : KnightSingleton<KnightZhConverter> {

	// Use this for initialization
	void Start () {
			
		}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Test()
	{
		Debug.Log("Test : " + ZhConverter.Convert ("旺福/阿爸我要當歌星旺福2015最新專輯阿 爸 我 要 當 歌 星史上最夏流的專輯自由 奔放 無拘 無束 無法 無天 天下無雙2015最新的旺福作業系統彎的音樂正式發表旺福來了當知道旺福已經成軍17年時全世界都震驚了是的旺福已經17歲再過一年 就可以去考駕照多年的經歷 海洋音樂祭獲獎三次金曲入圍最佳樂團 金音獎入圍與多次中華音樂人交流協會最佳專輯的肯定美國德州SXSW南方音樂節唯一亞洲樂團連續四年獲邀演出美國 加拿大 日本 新加坡 馬來西亞 大量的巡迴演出經驗與資歷", ZhConverter.To.Simplified));
	}
}
