using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Script : MonoBehaviour
{
	const uint BIT_ONE = (uint)1;

	Toggle[] toggles;

	bool sendCallback = true;

	void Start()
	{
		toggles = GameObject.FindObjectsOfType<Toggle>();

		foreach (var item in toggles)
			item.onValueChanged.AddListener(GetToggleHandler(item));

		System.Array.Sort(toggles, (lhs, rhs) => { return lhs.transform.GetSiblingIndex().CompareTo(rhs.transform.GetSiblingIndex()); });

		if (Application.isWebPlayer)
		{
			Application.ExternalCall("Start", gameObject.name);
			Application.ExternalCall("GetHash");
		}
	}

	private UnityEngine.Events.UnityAction<bool> GetToggleHandler(Toggle item)
	{
		return new UnityEngine.Events.UnityAction<bool>((b) =>
		{
			if (sendCallback)
				Application.ExternalCall("PushState", Encode());
		});
	}

	void GetHash(string hash)
	{
		Decode(hash);
	}

	private void Decode(string hash)
	{
		sendCallback = false;

		var values = hash.Split(',');
		uint bits, bit;

		for (int i = 0, index; i < values.Length; i++)
		{
			if (!UInt32.TryParse(values[i], out bits))
				continue;

			for (int j = 0; j < 32; j++)
			{
				bit = bits & BIT_ONE;
				bits = bits >> 1;

				// i=0, j=0 => index = 31
				// i=0, j=31 => index = 0
				// i=1, j=0 => index = 63
				index = i * 32 + (32 - j - 1);

				toggles[index].isOn = bit == BIT_ONE;
			}
		}

		sendCallback = true;
	}

	private string Encode()
	{
		List<uint> list = new List<uint>();
		for (int i = 0, index; i < toggles.Length; i++)
		{
			index = i / 32;
			if (i % 32 == 0)
				list.Add(0);

			list[index] = list[index] << 1;

			if (toggles[i].isOn)
				list[index] = list[index] | BIT_ONE;
		}
		return string.Join(",", list.ConvertAll(d => d.ToString()).ToArray());
	}

#if UNITY_EDITOR
	public bool DoEncode = false;
	public bool DoDecode = false;

	public string output;

	void Update()
	{
		if (DoEncode)
		{
			DoEncode = false;
			output = Encode();
			Debug.Log(output);
		}

		if (DoDecode)
		{
			DoDecode = false;
			Decode(output);
		}
	}
#endif
}