﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetStudio
{
    public sealed class SkinnedMeshRenderer : Renderer
    {
        public PPtr<Mesh> m_Mesh;
        public PPtr<Transform>[] m_Bones;
        public List<float> m_BlendShapeWeights;

        public SkinnedMeshRenderer(ObjectReader reader) : base(reader)
        {
            int m_Quality = reader.ReadInt32();
            var m_UpdateWhenOffscreen = reader.ReadBoolean();
            var m_SkinNormals = reader.ReadBoolean(); //3.1.0 and below
            reader.AlignStream(4);

            if (version[0] == 2 && version[1] < 6)//2.6 down
            {
                var m_DisableAnimationWhenOffscreen = new PPtr<Animation>(reader);
            }

            m_Mesh = new PPtr<Mesh>(reader);

            m_Bones = new PPtr<Transform>[reader.ReadInt32()];
            for (int b = 0; b < m_Bones.Length; b++)
            {
                m_Bones[b] = new PPtr<Transform>(reader);
            }

            if (version[0] < 3)
            {
                int m_BindPose = reader.ReadInt32();
                reader.Position += m_BindPose * 16 * 4;//Matrix4x4f
            }
            else
            {
                if (version[0] > 4 || (version[0] == 4 && version[1] >= 3))//4.3 and up
                {
                    int numBSWeights = reader.ReadInt32();
                    m_BlendShapeWeights = new List<float>(numBSWeights);
                    for (int i = 0; i < numBSWeights; i++)
                    {
                        m_BlendShapeWeights.Add(reader.ReadSingle());
                    }
                }
            }
        }
    }
}
