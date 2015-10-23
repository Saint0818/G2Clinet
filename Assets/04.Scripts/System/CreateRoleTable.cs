using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class CreateRoleTable
{
    private static readonly CreateRoleTable INSTANCE = new CreateRoleTable();
    public static CreateRoleTable Ins
    {
        get { return INSTANCE; }
    }

    /// <summary>
    /// Value: Item ID.
    /// </summary>
    private readonly Dictionary<EPlayerPostion, List<int>> mBodies = new Dictionary<EPlayerPostion, List<int>>();
    private readonly Dictionary<EPlayerPostion, List<int>> mHairs = new Dictionary<EPlayerPostion, List<int>>();
    private readonly Dictionary<EPlayerPostion, List<int>> mCloths = new Dictionary<EPlayerPostion, List<int>>();
    private readonly Dictionary<EPlayerPostion, List<int>> mPants = new Dictionary<EPlayerPostion, List<int>>();
    private readonly Dictionary<EPlayerPostion, List<int>> mShoes = new Dictionary<EPlayerPostion, List<int>>();

    private CreateRoleTable()
    {
    }

    public void Load(string jsonText)
    {
        Clear();

        TCreateRoleItems[] items = (TCreateRoleItems[])JsonConvert.DeserializeObject(jsonText, typeof(TCreateRoleItems[]));
        foreach(var item in items)
        {
            mBodies[EPlayerPostion.C].Add(item.ColorC);
            mHairs[EPlayerPostion.C].Add(item.HairC);
            mCloths[EPlayerPostion.C].Add(item.ClothC);
            mPants[EPlayerPostion.C].Add(item.PantsC);
            mShoes[EPlayerPostion.C].Add(item.ShoesC);

            mBodies[EPlayerPostion.F].Add(item.ColorF);
            mHairs[EPlayerPostion.F].Add(item.HairF);
            mCloths[EPlayerPostion.F].Add(item.ClothF);
            mPants[EPlayerPostion.F].Add(item.PantsF);
            mShoes[EPlayerPostion.F].Add(item.ShoesF);

            mBodies[EPlayerPostion.G].Add(item.ColorG);
            mHairs[EPlayerPostion.G].Add(item.HairG);
            mCloths[EPlayerPostion.G].Add(item.ClothG);
            mPants[EPlayerPostion.G].Add(item.PantsG);
            mShoes[EPlayerPostion.G].Add(item.ShoesG);
        }

        Debug.Log("[createroleitem parsed finished.]");
    }

    public void Clear()
    {
        mBodies.Clear();
        mHairs.Clear();
        mCloths.Clear();
        mPants.Clear();
        mShoes.Clear();

        foreach(EPlayerPostion pos in Enum.GetValues(typeof(EPlayerPostion)))
        {
            mBodies.Add(pos, new List<int>());
            mHairs.Add(pos, new List<int>());
            mCloths.Add(pos, new List<int>());
            mPants.Add(pos, new List<int>());
            mShoes.Add(pos, new List<int>());
        }
    }

    public int[] GetBody(EPlayerPostion pos)
    {
        return mBodies[pos].ToArray();
    }

    public int[] GetHairs(EPlayerPostion pos)
    {
        return mHairs[pos].ToArray();
    }

    public int[] GetCloths(EPlayerPostion pos)
    {
        return mCloths[pos].ToArray();
    }

    public int[] GetPants(EPlayerPostion pos)
    {
        return mPants[pos].ToArray();
    }

    public int[] GetShoes(EPlayerPostion pos)
    {
        return mShoes[pos].ToArray();
    }

	public int GetBodyCount(EPlayerPostion pos)
	{
		return mBodies[pos].Count;
	}
	
	public int GetHairsCount(EPlayerPostion pos)
	{
		return mHairs[pos].Count;
	}
	
	public int GetClothsCount(EPlayerPostion pos)
	{
		return mCloths[pos].Count;
	}
	
	public int GetPantsCount(EPlayerPostion pos)
	{
		return mPants[pos].Count;
	}
	
	public int GetShoesCount(EPlayerPostion pos)
	{
		return mShoes[pos].Count;
	}

	public int Body(EPlayerPostion pos, int index)
	{
		if (index >= 0 && index < mBodies[pos].Count)
			return mBodies[pos][index];
		else
			return 0;
	}
	
	public int Hair(EPlayerPostion pos, int index)
	{
		if (index >= 0 && index < mHairs[pos].Count)
			return mHairs[pos][index];
		else
			return 0;
	}
	
	public int Cloth(EPlayerPostion pos, int index)
	{
		if (index >= 0 && index < mCloths[pos].Count)
			return mCloths[pos][index];
		else
			return 0;
	}
	
	public int Pants(EPlayerPostion pos, int index)
	{
		if (index >= 0 && index < mPants[pos].Count)
			return mPants[pos][index];
		else
			return 0;
	}
	
	public int Shoes(EPlayerPostion pos, int index)
	{
		if (index >= 0 && index < mShoes[pos].Count)
			return mShoes[pos][index];
		else
			return 0;
	}
}