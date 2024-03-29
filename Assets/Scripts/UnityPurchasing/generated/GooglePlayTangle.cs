// WARNING: Do not modify! Generated file.
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS)
namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("/X5wf0/9fnV9/X5+f+Nh34nmaQTtuX8wTsH5LR4uWwthgAkZmSEpcYOII2E9CWNe0qJAnkLIePALRB4xT/1+XU9yeXZV+Tf5iHJ+fn56f3zsXiFgWXt9JhIou7Qxvsj2QbJKDB9sSXC5pxnp6LjojrL86hX8ZAzStbcj4vHlD975yCrxlIJLxxoyHr1rYOmFXRgw5gV38TZxheVFI4/vmMZlayMhVO6kLfFC0CpHu9O+eFwodObURMPxWP4ZCtrO6F6bEfqx1p1wtwuh9+qGxVwLKD1Ng34jbBcn0NjsRNEb7G13gYXTWYEHBx+XeDMJbIeYrtqS9+7LjUKj+i9qcHN8sNp+sAcfohldTipq87pDiQ8yju7PzfWxsh01BLNqkn18fn9+");
        private static int[] order = new int[] { 1,5,13,5,13,5,8,11,8,13,10,11,13,13,14 };
        private static int key = 127;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif