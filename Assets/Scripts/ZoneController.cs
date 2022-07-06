using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ZoneController : MonoBehaviour {

	[SerializeField] private Color purchasedColor;
	[SerializeField, Range(0, 100)] private int price;
	[SerializeField] private TextMeshPro priceText;
	[SerializeField] private bool isSpawnHelper;
	[SerializeField] private HelperController helperController;
	public bool IsOnSale { get; set; }
	public bool IsPurchased { get; set; } = false;
	private int priceLeftToBuy;

	public void Arrange()
	{
		priceLeftToBuy = price;
		IsOnSale = true;
		UpdatePriceText();
	}

	public void EvaluatePrice()
	{
		if (IsPurchased)
			return;

		priceLeftToBuy--;
		if (priceLeftToBuy == 0)
		{
			Purchased();
			return;
		}

		UpdatePriceText();

	}

	void UpdatePriceText() => priceText.text = priceLeftToBuy.ToString();

	void Purchased()
	{
		priceText.text = "PURCHASED";
		IsOnSale = false;
		IsPurchased = true;
		GetComponent<MeshRenderer>().material.color = purchasedColor;
		ZoneManager.Instance.IndexIncrement();
		ZoneManager.Instance.UnlockSpawnArea(this);
		if (!isSpawnHelper)
			return;

		helperController.gameObject.SetActive(true);
		helperController.IsAvailable = true;
		helperController.transform.DOMoveY(.5f, 1f).OnComplete(() =>
		{
			HelperManager.Instance.ArrangeTransportation(helperController);
		});
	}



}