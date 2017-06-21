using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;

namespace Avatars.AvatarComponents
{
    public static class AvatarManager
    {
        #region Field Region

        private static Dictionary<string, Avatar> avatarList = new Dictionary<string, Avatar>();

        #endregion

        #region Property Region

        public static Dictionary<string, Avatar> AvatarList
        {
            get { return avatarList; }
        }

        #endregion

        #region Constructor Region

        #endregion

        #region Method Region

        public static void AddAvatar(string name, Avatar avatar)
        {
            if (!avatarList.ContainsKey(name))
                avatarList.Add(name, avatar);
        }

        public static Avatar GetAvatar(string name)
        {
            if (avatarList.ContainsKey(name))
                return (Avatar)avatarList[name].Clone();

            return null;
        }

        public static void FromFile(string fileName, ContentManager content)
        {
            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    using (TextReader reader = new StreamReader(stream))
                    {
                        try
                        {
                            string lineIn = "";

                            do
                            {
                                lineIn = reader.ReadLine();
                                if (lineIn != null)
                                {
                                    Avatar avatar = Avatar.FromString(lineIn, content);
                                    if (!avatarList.ContainsKey(avatar.Name.ToLowerInvariant()))
                                        avatarList.Add(avatar.Name.ToLowerInvariant(), avatar);
                                }
                            } while (lineIn != null);
                        }
                        catch
                        {
                        }
                        finally
                        {
                            if (reader != null)
                                reader.Close();
                        }
                    }
                }
                catch
                {
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }
            }
        }

        #endregion
    }
}