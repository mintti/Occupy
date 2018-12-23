using UnityEngine;

namespace UnityStandardAssets.Utility
{
	public class SmoothFollow : MonoBehaviour
	{

		// The target we are following
		[SerializeField]
		private Transform target;


        private Vector3 targetPos;
        private Vector3 Pos;
		void Start() {
            Pos = transform.position;
        }

		// Update is called once per frame
		void LateUpdate()
		{
            targetPos = target.position;
            transform.position = targetPos + Pos;
            
		}
	}
}