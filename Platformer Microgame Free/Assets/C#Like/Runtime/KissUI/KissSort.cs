/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CSharpLike
{
    [ExecuteInEditMode]
	/// <summary>
	/// Sort the GameObject by the name string
	/// </summary>
	public class KissSort : MonoBehaviour
	{
		/// <summary>
		/// Sort ASC or DESC
		/// </summary>
		public bool sortAsc = true;
		/// <summary>
		/// Mark it need to sort, will be sort in Update function
		/// </summary>
		public bool MarkNeedSort = false;

		void Update()
		{
			if (MarkNeedSort)
            {
				SortNow();
			}
		}
		public void SortNow()
		{
			MarkNeedSort = false;
			int count = transform.childCount;
			if (count <= 0)
				return;
			List<Transform> childs = new List<Transform>();
			foreach(Transform t in transform)
				childs.Add(t);

			if (sortAsc)
				childs.Sort(SortASC);
			else
				childs.Sort(SortDESC);

			for (int i = 0; i < count; i++)
				childs[i].SetSiblingIndex(i);
		}
		int SortASC(Transform a, Transform b)
        {
			return string.Compare(a.name, b.name);
		}
		int SortDESC(Transform a, Transform b)
		{
			return string.Compare(b.name, a.name);
		}
	}
}

