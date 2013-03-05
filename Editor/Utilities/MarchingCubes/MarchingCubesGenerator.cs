using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework;


namespace AweEditor.Utilities.MarchingCubes
{
    class MarchingCubesGenerator
    {
        /// <summary>
        /// Look up table taken from http://paulbourke.net/geometry/polygonise/
        /// </summary>
        #region Look up table

        const int[,] TRI_TABLE = 
			{{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{0, 1, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{1, 8, 3, 9, 8, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{0, 8, 3, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{9, 2, 10, 0, 2, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{2, 8, 3, 2, 10, 8, 10, 9, 8, -1, -1, -1, -1, -1, -1, -1},
			{3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{0, 11, 2, 8, 11, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{1, 9, 0, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{1, 11, 2, 1, 9, 11, 9, 8, 11, -1, -1, -1, -1, -1, -1, -1},
			{3, 10, 1, 11, 10, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{0, 10, 1, 0, 8, 10, 8, 11, 10, -1, -1, -1, -1, -1, -1, -1},
			{3, 9, 0, 3, 11, 9, 11, 10, 9, -1, -1, -1, -1, -1, -1, -1},
			{9, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{4, 3, 0, 7, 3, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{0, 1, 9, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{4, 1, 9, 4, 7, 1, 7, 3, 1, -1, -1, -1, -1, -1, -1, -1},
			{1, 2, 10, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{3, 4, 7, 3, 0, 4, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1},
			{9, 2, 10, 9, 0, 2, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},
			{2, 10, 9, 2, 9, 7, 2, 7, 3, 7, 9, 4, -1, -1, -1, -1},
			{8, 4, 7, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{11, 4, 7, 11, 2, 4, 2, 0, 4, -1, -1, -1, -1, -1, -1, -1},
			{9, 0, 1, 8, 4, 7, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},
			{4, 7, 11, 9, 4, 11, 9, 11, 2, 9, 2, 1, -1, -1, -1, -1},
			{3, 10, 1, 3, 11, 10, 7, 8, 4, -1, -1, -1, -1, -1, -1, -1},
			{1, 11, 10, 1, 4, 11, 1, 0, 4, 7, 11, 4, -1, -1, -1, -1},
			{4, 7, 8, 9, 0, 11, 9, 11, 10, 11, 0, 3, -1, -1, -1, -1},
			{4, 7, 11, 4, 11, 9, 9, 11, 10, -1, -1, -1, -1, -1, -1, -1},
			{9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{9, 5, 4, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{0, 5, 4, 1, 5, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{8, 5, 4, 8, 3, 5, 3, 1, 5, -1, -1, -1, -1, -1, -1, -1},
			{1, 2, 10, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{3, 0, 8, 1, 2, 10, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},
			{5, 2, 10, 5, 4, 2, 4, 0, 2, -1, -1, -1, -1, -1, -1, -1},
			{2, 10, 5, 3, 2, 5, 3, 5, 4, 3, 4, 8, -1, -1, -1, -1},
			{9, 5, 4, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{0, 11, 2, 0, 8, 11, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},
			{0, 5, 4, 0, 1, 5, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},
			{2, 1, 5, 2, 5, 8, 2, 8, 11, 4, 8, 5, -1, -1, -1, -1},
			{10, 3, 11, 10, 1, 3, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1},
			{4, 9, 5, 0, 8, 1, 8, 10, 1, 8, 11, 10, -1, -1, -1, -1},
			{5, 4, 0, 5, 0, 11, 5, 11, 10, 11, 0, 3, -1, -1, -1, -1},
			{5, 4, 8, 5, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1},
			{9, 7, 8, 5, 7, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{9, 3, 0, 9, 5, 3, 5, 7, 3, -1, -1, -1, -1, -1, -1, -1},
			{0, 7, 8, 0, 1, 7, 1, 5, 7, -1, -1, -1, -1, -1, -1, -1},
			{1, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{9, 7, 8, 9, 5, 7, 10, 1, 2, -1, -1, -1, -1, -1, -1, -1},
			{10, 1, 2, 9, 5, 0, 5, 3, 0, 5, 7, 3, -1, -1, -1, -1},
			{8, 0, 2, 8, 2, 5, 8, 5, 7, 10, 5, 2, -1, -1, -1, -1},
			{2, 10, 5, 2, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1},
			{7, 9, 5, 7, 8, 9, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1},
			{9, 5, 7, 9, 7, 2, 9, 2, 0, 2, 7, 11, -1, -1, -1, -1},
			{2, 3, 11, 0, 1, 8, 1, 7, 8, 1, 5, 7, -1, -1, -1, -1},
			{11, 2, 1, 11, 1, 7, 7, 1, 5, -1, -1, -1, -1, -1, -1, -1},
			{9, 5, 8, 8, 5, 7, 10, 1, 3, 10, 3, 11, -1, -1, -1, -1},
			{5, 7, 0, 5, 0, 9, 7, 11, 0, 1, 0, 10, 11, 10, 0, -1},
			{11, 10, 0, 11, 0, 3, 10, 5, 0, 8, 0, 7, 5, 7, 0, -1},
			{11, 10, 5, 7, 11, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{0, 8, 3, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{9, 0, 1, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{1, 8, 3, 1, 9, 8, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},
			{1, 6, 5, 2, 6, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{1, 6, 5, 1, 2, 6, 3, 0, 8, -1, -1, -1, -1, -1, -1, -1},
			{9, 6, 5, 9, 0, 6, 0, 2, 6, -1, -1, -1, -1, -1, -1, -1},
			{5, 9, 8, 5, 8, 2, 5, 2, 6, 3, 2, 8, -1, -1, -1, -1},
			{2, 3, 11, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{11, 0, 8, 11, 2, 0, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},
			{0, 1, 9, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},
			{5, 10, 6, 1, 9, 2, 9, 11, 2, 9, 8, 11, -1, -1, -1, -1},
			{6, 3, 11, 6, 5, 3, 5, 1, 3, -1, -1, -1, -1, -1, -1, -1},
			{0, 8, 11, 0, 11, 5, 0, 5, 1, 5, 11, 6, -1, -1, -1, -1},
			{3, 11, 6, 0, 3, 6, 0, 6, 5, 0, 5, 9, -1, -1, -1, -1},
			{6, 5, 9, 6, 9, 11, 11, 9, 8, -1, -1, -1, -1, -1, -1, -1},
			{5, 10, 6, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{4, 3, 0, 4, 7, 3, 6, 5, 10, -1, -1, -1, -1, -1, -1, -1},
			{1, 9, 0, 5, 10, 6, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},
			{10, 6, 5, 1, 9, 7, 1, 7, 3, 7, 9, 4, -1, -1, -1, -1},
			{6, 1, 2, 6, 5, 1, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1},
			{1, 2, 5, 5, 2, 6, 3, 0, 4, 3, 4, 7, -1, -1, -1, -1},
			{8, 4, 7, 9, 0, 5, 0, 6, 5, 0, 2, 6, -1, -1, -1, -1},
			{7, 3, 9, 7, 9, 4, 3, 2, 9, 5, 9, 6, 2, 6, 9, -1},
			{3, 11, 2, 7, 8, 4, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},
			{5, 10, 6, 4, 7, 2, 4, 2, 0, 2, 7, 11, -1, -1, -1, -1},
			{0, 1, 9, 4, 7, 8, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1},
			{9, 2, 1, 9, 11, 2, 9, 4, 11, 7, 11, 4, 5, 10, 6, -1},
			{8, 4, 7, 3, 11, 5, 3, 5, 1, 5, 11, 6, -1, -1, -1, -1},
			{5, 1, 11, 5, 11, 6, 1, 0, 11, 7, 11, 4, 0, 4, 11, -1},
			{0, 5, 9, 0, 6, 5, 0, 3, 6, 11, 6, 3, 8, 4, 7, -1},
			{6, 5, 9, 6, 9, 11, 4, 7, 9, 7, 11, 9, -1, -1, -1, -1},
			{10, 4, 9, 6, 4, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{4, 10, 6, 4, 9, 10, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1},
			{10, 0, 1, 10, 6, 0, 6, 4, 0, -1, -1, -1, -1, -1, -1, -1},
			{8, 3, 1, 8, 1, 6, 8, 6, 4, 6, 1, 10, -1, -1, -1, -1},
			{1, 4, 9, 1, 2, 4, 2, 6, 4, -1, -1, -1, -1, -1, -1, -1},
			{3, 0, 8, 1, 2, 9, 2, 4, 9, 2, 6, 4, -1, -1, -1, -1},
			{0, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{8, 3, 2, 8, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1},
			{10, 4, 9, 10, 6, 4, 11, 2, 3, -1, -1, -1, -1, -1, -1, -1},
			{0, 8, 2, 2, 8, 11, 4, 9, 10, 4, 10, 6, -1, -1, -1, -1},
			{3, 11, 2, 0, 1, 6, 0, 6, 4, 6, 1, 10, -1, -1, -1, -1},
			{6, 4, 1, 6, 1, 10, 4, 8, 1, 2, 1, 11, 8, 11, 1, -1},
			{9, 6, 4, 9, 3, 6, 9, 1, 3, 11, 6, 3, -1, -1, -1, -1},
			{8, 11, 1, 8, 1, 0, 11, 6, 1, 9, 1, 4, 6, 4, 1, -1},
			{3, 11, 6, 3, 6, 0, 0, 6, 4, -1, -1, -1, -1, -1, -1, -1},
			{6, 4, 8, 11, 6, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{7, 10, 6, 7, 8, 10, 8, 9, 10, -1, -1, -1, -1, -1, -1, -1},
			{0, 7, 3, 0, 10, 7, 0, 9, 10, 6, 7, 10, -1, -1, -1, -1},
			{10, 6, 7, 1, 10, 7, 1, 7, 8, 1, 8, 0, -1, -1, -1, -1},
			{10, 6, 7, 10, 7, 1, 1, 7, 3, -1, -1, -1, -1, -1, -1, -1},
			{1, 2, 6, 1, 6, 8, 1, 8, 9, 8, 6, 7, -1, -1, -1, -1},
			{2, 6, 9, 2, 9, 1, 6, 7, 9, 0, 9, 3, 7, 3, 9, -1},
			{7, 8, 0, 7, 0, 6, 6, 0, 2, -1, -1, -1, -1, -1, -1, -1},
			{7, 3, 2, 6, 7, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{2, 3, 11, 10, 6, 8, 10, 8, 9, 8, 6, 7, -1, -1, -1, -1},
			{2, 0, 7, 2, 7, 11, 0, 9, 7, 6, 7, 10, 9, 10, 7, -1},
			{1, 8, 0, 1, 7, 8, 1, 10, 7, 6, 7, 10, 2, 3, 11, -1},
			{11, 2, 1, 11, 1, 7, 10, 6, 1, 6, 7, 1, -1, -1, -1, -1},
			{8, 9, 6, 8, 6, 7, 9, 1, 6, 11, 6, 3, 1, 3, 6, -1},
			{0, 9, 1, 11, 6, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{7, 8, 0, 7, 0, 6, 3, 11, 0, 11, 6, 0, -1, -1, -1, -1},
			{7, 11, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{3, 0, 8, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{0, 1, 9, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{8, 1, 9, 8, 3, 1, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},
			{10, 1, 2, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{1, 2, 10, 3, 0, 8, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},
			{2, 9, 0, 2, 10, 9, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},
			{6, 11, 7, 2, 10, 3, 10, 8, 3, 10, 9, 8, -1, -1, -1, -1},
			{7, 2, 3, 6, 2, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{7, 0, 8, 7, 6, 0, 6, 2, 0, -1, -1, -1, -1, -1, -1, -1},
			{2, 7, 6, 2, 3, 7, 0, 1, 9, -1, -1, -1, -1, -1, -1, -1},
			{1, 6, 2, 1, 8, 6, 1, 9, 8, 8, 7, 6, -1, -1, -1, -1},
			{10, 7, 6, 10, 1, 7, 1, 3, 7, -1, -1, -1, -1, -1, -1, -1},
			{10, 7, 6, 1, 7, 10, 1, 8, 7, 1, 0, 8, -1, -1, -1, -1},
			{0, 3, 7, 0, 7, 10, 0, 10, 9, 6, 10, 7, -1, -1, -1, -1},
			{7, 6, 10, 7, 10, 8, 8, 10, 9, -1, -1, -1, -1, -1, -1, -1},
			{6, 8, 4, 11, 8, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{3, 6, 11, 3, 0, 6, 0, 4, 6, -1, -1, -1, -1, -1, -1, -1},
			{8, 6, 11, 8, 4, 6, 9, 0, 1, -1, -1, -1, -1, -1, -1, -1},
			{9, 4, 6, 9, 6, 3, 9, 3, 1, 11, 3, 6, -1, -1, -1, -1},
			{6, 8, 4, 6, 11, 8, 2, 10, 1, -1, -1, -1, -1, -1, -1, -1},
			{1, 2, 10, 3, 0, 11, 0, 6, 11, 0, 4, 6, -1, -1, -1, -1},
			{4, 11, 8, 4, 6, 11, 0, 2, 9, 2, 10, 9, -1, -1, -1, -1},
			{10, 9, 3, 10, 3, 2, 9, 4, 3, 11, 3, 6, 4, 6, 3, -1},
			{8, 2, 3, 8, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1},
			{0, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{1, 9, 0, 2, 3, 4, 2, 4, 6, 4, 3, 8, -1, -1, -1, -1},
			{1, 9, 4, 1, 4, 2, 2, 4, 6, -1, -1, -1, -1, -1, -1, -1},
			{8, 1, 3, 8, 6, 1, 8, 4, 6, 6, 10, 1, -1, -1, -1, -1},
			{10, 1, 0, 10, 0, 6, 6, 0, 4, -1, -1, -1, -1, -1, -1, -1},
			{4, 6, 3, 4, 3, 8, 6, 10, 3, 0, 3, 9, 10, 9, 3, -1},
			{10, 9, 4, 6, 10, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{4, 9, 5, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{0, 8, 3, 4, 9, 5, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},
			{5, 0, 1, 5, 4, 0, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},
			{11, 7, 6, 8, 3, 4, 3, 5, 4, 3, 1, 5, -1, -1, -1, -1},
			{9, 5, 4, 10, 1, 2, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},
			{6, 11, 7, 1, 2, 10, 0, 8, 3, 4, 9, 5, -1, -1, -1, -1},
			{7, 6, 11, 5, 4, 10, 4, 2, 10, 4, 0, 2, -1, -1, -1, -1},
			{3, 4, 8, 3, 5, 4, 3, 2, 5, 10, 5, 2, 11, 7, 6, -1},
			{7, 2, 3, 7, 6, 2, 5, 4, 9, -1, -1, -1, -1, -1, -1, -1},
			{9, 5, 4, 0, 8, 6, 0, 6, 2, 6, 8, 7, -1, -1, -1, -1},
			{3, 6, 2, 3, 7, 6, 1, 5, 0, 5, 4, 0, -1, -1, -1, -1},
			{6, 2, 8, 6, 8, 7, 2, 1, 8, 4, 8, 5, 1, 5, 8, -1},
			{9, 5, 4, 10, 1, 6, 1, 7, 6, 1, 3, 7, -1, -1, -1, -1},
			{1, 6, 10, 1, 7, 6, 1, 0, 7, 8, 7, 0, 9, 5, 4, -1},
			{4, 0, 10, 4, 10, 5, 0, 3, 10, 6, 10, 7, 3, 7, 10, -1},
			{7, 6, 10, 7, 10, 8, 5, 4, 10, 4, 8, 10, -1, -1, -1, -1},
			{6, 9, 5, 6, 11, 9, 11, 8, 9, -1, -1, -1, -1, -1, -1, -1},
			{3, 6, 11, 0, 6, 3, 0, 5, 6, 0, 9, 5, -1, -1, -1, -1},
			{0, 11, 8, 0, 5, 11, 0, 1, 5, 5, 6, 11, -1, -1, -1, -1},
			{6, 11, 3, 6, 3, 5, 5, 3, 1, -1, -1, -1, -1, -1, -1, -1},
			{1, 2, 10, 9, 5, 11, 9, 11, 8, 11, 5, 6, -1, -1, -1, -1},
			{0, 11, 3, 0, 6, 11, 0, 9, 6, 5, 6, 9, 1, 2, 10, -1},
			{11, 8, 5, 11, 5, 6, 8, 0, 5, 10, 5, 2, 0, 2, 5, -1},
			{6, 11, 3, 6, 3, 5, 2, 10, 3, 10, 5, 3, -1, -1, -1, -1},
			{5, 8, 9, 5, 2, 8, 5, 6, 2, 3, 8, 2, -1, -1, -1, -1},
			{9, 5, 6, 9, 6, 0, 0, 6, 2, -1, -1, -1, -1, -1, -1, -1},
			{1, 5, 8, 1, 8, 0, 5, 6, 8, 3, 8, 2, 6, 2, 8, -1},
			{1, 5, 6, 2, 1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{1, 3, 6, 1, 6, 10, 3, 8, 6, 5, 6, 9, 8, 9, 6, -1},
			{10, 1, 0, 10, 0, 6, 9, 5, 0, 5, 6, 0, -1, -1, -1, -1},
			{0, 3, 8, 5, 6, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{10, 5, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{11, 5, 10, 7, 5, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{11, 5, 10, 11, 7, 5, 8, 3, 0, -1, -1, -1, -1, -1, -1, -1},
			{5, 11, 7, 5, 10, 11, 1, 9, 0, -1, -1, -1, -1, -1, -1, -1},
			{10, 7, 5, 10, 11, 7, 9, 8, 1, 8, 3, 1, -1, -1, -1, -1},
			{11, 1, 2, 11, 7, 1, 7, 5, 1, -1, -1, -1, -1, -1, -1, -1},
			{0, 8, 3, 1, 2, 7, 1, 7, 5, 7, 2, 11, -1, -1, -1, -1},
			{9, 7, 5, 9, 2, 7, 9, 0, 2, 2, 11, 7, -1, -1, -1, -1},
			{7, 5, 2, 7, 2, 11, 5, 9, 2, 3, 2, 8, 9, 8, 2, -1},
			{2, 5, 10, 2, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1},
			{8, 2, 0, 8, 5, 2, 8, 7, 5, 10, 2, 5, -1, -1, -1, -1},
			{9, 0, 1, 5, 10, 3, 5, 3, 7, 3, 10, 2, -1, -1, -1, -1},
			{9, 8, 2, 9, 2, 1, 8, 7, 2, 10, 2, 5, 7, 5, 2, -1},
			{1, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{0, 8, 7, 0, 7, 1, 1, 7, 5, -1, -1, -1, -1, -1, -1, -1},
			{9, 0, 3, 9, 3, 5, 5, 3, 7, -1, -1, -1, -1, -1, -1, -1},
			{9, 8, 7, 5, 9, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{5, 8, 4, 5, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1},
			{5, 0, 4, 5, 11, 0, 5, 10, 11, 11, 3, 0, -1, -1, -1, -1},
			{0, 1, 9, 8, 4, 10, 8, 10, 11, 10, 4, 5, -1, -1, -1, -1},
			{10, 11, 4, 10, 4, 5, 11, 3, 4, 9, 4, 1, 3, 1, 4, -1},
			{2, 5, 1, 2, 8, 5, 2, 11, 8, 4, 5, 8, -1, -1, -1, -1},
			{0, 4, 11, 0, 11, 3, 4, 5, 11, 2, 11, 1, 5, 1, 11, -1},
			{0, 2, 5, 0, 5, 9, 2, 11, 5, 4, 5, 8, 11, 8, 5, -1},
			{9, 4, 5, 2, 11, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{2, 5, 10, 3, 5, 2, 3, 4, 5, 3, 8, 4, -1, -1, -1, -1},
			{5, 10, 2, 5, 2, 4, 4, 2, 0, -1, -1, -1, -1, -1, -1, -1},
			{3, 10, 2, 3, 5, 10, 3, 8, 5, 4, 5, 8, 0, 1, 9, -1},
			{5, 10, 2, 5, 2, 4, 1, 9, 2, 9, 4, 2, -1, -1, -1, -1},
			{8, 4, 5, 8, 5, 3, 3, 5, 1, -1, -1, -1, -1, -1, -1, -1},
			{0, 4, 5, 1, 0, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{8, 4, 5, 8, 5, 3, 9, 0, 5, 0, 3, 5, -1, -1, -1, -1},
			{9, 4, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{4, 11, 7, 4, 9, 11, 9, 10, 11, -1, -1, -1, -1, -1, -1, -1},
			{0, 8, 3, 4, 9, 7, 9, 11, 7, 9, 10, 11, -1, -1, -1, -1},
			{1, 10, 11, 1, 11, 4, 1, 4, 0, 7, 4, 11, -1, -1, -1, -1},
			{3, 1, 4, 3, 4, 8, 1, 10, 4, 7, 4, 11, 10, 11, 4, -1},
			{4, 11, 7, 9, 11, 4, 9, 2, 11, 9, 1, 2, -1, -1, -1, -1},
			{9, 7, 4, 9, 11, 7, 9, 1, 11, 2, 11, 1, 0, 8, 3, -1},
			{11, 7, 4, 11, 4, 2, 2, 4, 0, -1, -1, -1, -1, -1, -1, -1},
			{11, 7, 4, 11, 4, 2, 8, 3, 4, 3, 2, 4, -1, -1, -1, -1},
			{2, 9, 10, 2, 7, 9, 2, 3, 7, 7, 4, 9, -1, -1, -1, -1},
			{9, 10, 7, 9, 7, 4, 10, 2, 7, 8, 7, 0, 2, 0, 7, -1},
			{3, 7, 10, 3, 10, 2, 7, 4, 10, 1, 10, 0, 4, 0, 10, -1},
			{1, 10, 2, 8, 7, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{4, 9, 1, 4, 1, 7, 7, 1, 3, -1, -1, -1, -1, -1, -1, -1},
			{4, 9, 1, 4, 1, 7, 0, 8, 1, 8, 7, 1, -1, -1, -1, -1},
			{4, 0, 3, 7, 4, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{4, 8, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{9, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{3, 0, 9, 3, 9, 11, 11, 9, 10, -1, -1, -1, -1, -1, -1, -1},
			{0, 1, 10, 0, 10, 8, 8, 10, 11, -1, -1, -1, -1, -1, -1, -1},
			{3, 1, 10, 11, 3, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{1, 2, 11, 1, 11, 9, 9, 11, 8, -1, -1, -1, -1, -1, -1, -1},
			{3, 0, 9, 3, 9, 11, 1, 2, 9, 2, 11, 9, -1, -1, -1, -1},
			{0, 2, 11, 8, 0, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{3, 2, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{2, 3, 8, 2, 8, 10, 10, 8, 9, -1, -1, -1, -1, -1, -1, -1},
			{9, 10, 2, 0, 9, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{2, 3, 8, 2, 8, 10, 0, 1, 8, 1, 10, 8, -1, -1, -1, -1},
			{1, 10, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{1, 3, 8, 9, 1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{0, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{0, 3, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};


        #endregion
        private int WORLD_WIDTH;
        private int WORLD_LENGTH;
        private int WORLD_HEIGHT;

        public string MeshName { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MarchingCubesGenerator()
            : this(512, 256, 512, "Default")
        {
        }

        /// <summary>
        /// Constructors taking world dimensions and a meshname
        /// </summary>
        /// <param name="x">world width</param>
        /// <param name="y">world height</param>
        /// <param name="z">world length</param>
        /// <param name="meshName">mesh name</param>
        public MarchingCubesGenerator(int x, int y, int z, string meshName)
        {
            WORLD_WIDTH = x;
            WORLD_HEIGHT = y;
            WORLD_LENGTH = z;
            MeshName = meshName;
        }

        /// <summary>
        /// Takes in a list of blocks and whether or not the mesh will be textured
        /// </summary>
        /// <param name="blockList">List of blocks</param>
        /// <param name="simple">Textured or not</param>
        /// <returns>a mesh generated from the list of blockData</returns>
        public MeshContent March(List<BlockData> blockList, bool simple)
        {
            MeshBuilder mBuilder = MeshBuilder.StartMesh(MeshName);
            byte[,] slice1 = GetSlice(blockList, 0, simple);
            byte[,] slice2 = GetSlice(blockList, 1, simple);
            byte[,] slice3 = GetSlice(blockList, 2, simple);
            byte[,] slice4 = GetSlice(blockList, 3, simple);
            byte[,] index;
            Tuple<byte, byte[]>[,] tIndex;

            for (int z = 0; z < WORLD_LENGTH - 1; z++)
            {
                if (simple)
                {
                    index = ProcessSlicesV2(slice1, slice2);
                    AddToMesh(mBuilder, index, z);
                }
                else
                {
                    tIndex = ProcessTexturedSlices(slice1, slice2);
                    AddToTexturedMesh(tIndex, z);
                }

                slice1 = slice2;
                slice2 = slice3;
                slice3 = slice4;
                slice4 = GetSlice(blockList, z + 4, simple);
            }

            return mBuilder.FinishMesh();
        }

        /// <summary>
        /// Gets a slice of the world from the blocklist at the given z coordinate,
        /// </summary>
        /// <param name="blockList">list of block data in the world</param>
        /// <param name="z">z coordinate of the slice in the world</param>
        /// <param name="simple">whether or not the mesh will be textured</param>
        /// <returns>a 2 dimensional array that represents a X by Y slice of the world at the given z coordinate</returns>
        private byte[,] GetSlice(List<BlockData> blockList, int z, bool simple)
        {
            if (z > WORLD_LENGTH)
                return null;

            byte[,] slice = new byte[WORLD_WIDTH, WORLD_HEIGHT];
            List<BlockData> list = blockList.FindAll(x => x.z == z);

            if (simple)
            {
                list.ForEach(block => slice[block.x, block.y] = 1);
            }
            else
            {
                list.ForEach(block => slice[block.x, block.y] = block.Type);
            }

            return slice;
        }

        /// <summary>
        /// Generates an array of byte indicies from the 2 given slices
        /// </summary>
        /// <param name="slice1">the first slice</param>
        /// <param name="slice2">the second slice</param>
        /// <returns>an array of byte indicies based on the 2 slices</returns>
        private byte[,] ProcessSlicesV2(byte[,] slice1, byte[,] slice2)
        {
            byte[,] index = new byte[WORLD_WIDTH - 1, WORLD_HEIGHT - 1];
            byte v0, v1, v2, v3, v4, v5, v6, v7;

            for (int y = 0; y < WORLD_HEIGHT - 1; y++)
            {
                for (int x = 0; x < WORLD_WIDTH - 1; x++)
                {
                    if (slice1 == null)
                    {
                        v0 = 0;
                        v1 = 0;
                        v2 = slice2[x + 1, y + 1];
                        v3 = slice2[x, y + 1];
                        v4 = 0;
                        v5 = 0;
                        v6 = slice2[x + 1, y];
                        v7 = slice2[x, y];
                    }
                    else if (slice2 == null)
                    {
                        v0 = slice1[x, y + 1];
                        v1 = slice1[x + 1, y + 1];
                        v2 = 0;
                        v3 = 0;
                        v4 = slice1[x, y];
                        v5 = slice1[x + 1, y];
                        v6 = 0;
                        v7 = 0;
                    }
                    else
                    {
                        v0 = slice1[x, y + 1];
                        v1 = slice1[x + 1, y + 1];
                        v2 = slice2[x + 1, y + 1];
                        v3 = slice2[x, y + 1];
                        v4 = slice1[x, y];
                        v5 = slice1[x + 1, y];
                        v6 = slice2[x + 1, y];
                        v7 = slice2[x, y];
                    }

                    index[x, y] = (byte)((v0) | (v1 << 1) | (v2 << 2) | (v3 << 3) | (v4 << 4) | (v5 << 5) | (v6 << 6) | (v7 << 7));
                }
            }
            return index;
        }

        /// <summary>
        /// Adds the given byte indicies to the meshbuilder at the given z coordinate
        /// </summary>
        /// <param name="mBuilder">the meshBuilder</param>
        /// <param name="meshIndicies">the array of byte indicies</param>
        /// <param name="z">the z coordinate</param>
        private void AddToMesh(MeshBuilder mBuilder, byte[,] meshIndicies, int z)
        {
            byte[,] indicies = meshIndicies;

            for (int x = 0; x < WORLD_WIDTH - 1; x++)
            {
                for (int y = 0; y < WORLD_HEIGHT - 1; y++)
                {
                    ProcessIndex2(mBuilder, indicies[x, y], x, y, z);
                }
            }
        }

        /// <summary>
        /// Adds triangles given by the index to the meshbuilder at the given x,y,z coordinates
        /// </summary>
        /// <param name="mBuilder">the MeshBuilder</param>
        /// <param name="index">the byte index</param>
        /// <param name="x">the x coordinate</param>
        /// <param name="y">the y coordinate</param>
        /// <param name="z">the z coordinate</param>
        private void ProcessIndex2(MeshBuilder mBuilder, byte index, int x, int y, int z)
        {
            for (int i = 0; TRI_TABLE[index, i] != -1; i += 3)
            {
                mBuilder.AddTriangleVertex(mBuilder.CreatePosition(GetVertex2(TRI_TABLE[index, i], x, y, z)));
                mBuilder.AddTriangleVertex(mBuilder.CreatePosition(GetVertex2(TRI_TABLE[index, i + 1], x, y, z)));
                mBuilder.AddTriangleVertex(mBuilder.CreatePosition(GetVertex2(TRI_TABLE[index, i + 2], x, y, z)));
            }
        }

        /// <summary>
        /// Gets a vertex position from the given vertex number, and its x,y,z coordinate
        /// </summary>
        /// <param name="vertex">the vertex number</param>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <param name="z">z coordinate</param>
        /// <returns>Vector position</returns>
        private Vector3 GetVertex2(int vertex, int x, int y, int z)
        {
            switch (vertex)
            {
                case 0:
                    return new Vector3(2 * x + 1, 2 * y + 2, 2 * z);
                case 1:
                    return new Vector3(2 * x + 2, 2 * y + 2, 2 * z + 1);
                case 2:
                    return new Vector3(2 * x + 1, 2 * y + 2, 2 * z + 2);
                case 3:
                    return new Vector3(2 * x, 2 * y + 2, 2 * z + 1);
                case 4:
                    return new Vector3(2 * x + 1, 2 * y, 2 * z);
                case 5:
                    return new Vector3(2 * x + 2, 2 * y, 2 * z + 1);
                case 6:
                    return new Vector3(2 * x + 1, 2 * y, 2 * z + 2);
                case 7:
                    return new Vector3(2 * x, 2 * y, 2 * z + 1);
                case 8:
                    return new Vector3(2 * x, 2 * y + 1, 2 * z);
                case 9:
                    return new Vector3(2 * x + 2, 2 * y + 1, 2 * z);
                case 10:
                    return new Vector3(2 * x + 2, 2 * y + 1, 2 * z + 2);
                case 11:
                    return new Vector3(2 * x, 2 * y + 1, 2 * z + 2);
                default:
                    throw new Exception("Unknown vertex");
            }
        }

        #region Non-working Code
        private byte[,] ProcessSlices(byte[,] slice1, byte[,] slice2)
        {
            byte[,] index = new byte[WORLD_WIDTH - 1, WORLD_HEIGHT - 1];
            byte v1, v2, v3, v4, v5, v6, v7, v8;

            for (int y = 0; y < WORLD_HEIGHT - 1; y++)
            {
                for (int x = 0; x < WORLD_WIDTH - 1; x++)
                {
                    if (slice1 == null)
                    {
                        v1 = 0;
                        v2 = 0;
                        v3 = 0;
                        v4 = 0;
                        v5 = slice2[x, y];
                        v6 = slice2[x + 1, y];
                        v7 = slice2[x + 1, y + 1];
                        v8 = slice2[x, y + 1];
                    }
                    else if (slice2 == null)
                    {
                        v1 = slice1[x, y];
                        v2 = slice1[x + 1, y];
                        v3 = slice1[x + 1, y + 1];
                        v4 = slice1[x, y + 1];
                        v5 = 0;
                        v6 = 0;
                        v7 = 0;
                        v8 = 0;
                    }
                    else
                    {
                        v1 = slice1[x, y];
                        v2 = slice1[x + 1, y];
                        v3 = slice1[x + 1, y + 1];
                        v4 = slice1[x, y + 1];
                        v5 = slice2[x, y];
                        v6 = slice2[x + 1, y];
                        v7 = slice2[x + 1, y + 1];
                        v8 = slice2[x, y + 1];
                    }
                    index[x, y] = (byte)((v1) | (v2 << 1) | (v3 << 2) | (v4 << 3) | (v5 << 4) | (v6 << 5) | (v7 << 6) | (v8 << 7));
                }
            }
            return index;
        }


        /// <summary>
        /// Adds triangle verticies to the MeshBuilder based on the index given.
        /// </summary>
        /// <param name="mBuilder"></param>
        /// <param name="index"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        private void ProcessIndex(MeshBuilder mBuilder, byte index, int x, int y, int z)
        {
            Vector3 e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12;
            e1 = GetVertex(1, x, y, z);
            e2 = GetVertex(2, x, y, z);
            e3 = GetVertex(3, x, y, z);
            e4 = GetVertex(4, x, y, z);
            e5 = GetVertex(5, x, y, z);
            e6 = GetVertex(6, x, y, z);
            e7 = GetVertex(7, x, y, z);
            e8 = GetVertex(8, x, y, z);
            e9 = GetVertex(9, x, y, z);
            e10 = GetVertex(10, x, y, z);
            e11 = GetVertex(11, x, y, z);
            e12 = GetVertex(12, x, y, z);

            switch (index)
            {
                //v8v7v6v5 v4v3v2v1
                #region 0-99
                #region 0-9
                case (0): //0000 0000
                    //Empty Cube, do nothing
                    break;
                case (1): //0000 0001
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e1));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e4));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e5));
                    break;
                case (2): //0000 0010
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e1));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e2));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e6));
                    break;
                case (3): //0000 0011
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e4));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e2));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e5));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e5));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e6));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e2));
                    break;
                case (4): //0000 0100
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e2));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e3));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e7));
                    break;
                case (5): //0000 0101
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e1));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e4));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e5));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e2));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e3));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e7));
                    break;
                case (6): //0000 0110
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e1));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e3));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e6));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e6));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e7));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e3));
                    break;
                case (7): //0000 0111
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e5));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e6));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e7));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e5));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e7));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e4));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e4));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e3));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e7));
                    break;
                case (8): //0000 1000
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e4));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e3));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e8));
                    break;
                case (9): //0000 1001
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e5));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e8));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e3));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e1));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e3));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e5));
                    break;
                #endregion;
                #region 10-19
                case (10): //0000 1010
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e4));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e3));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e8));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e1));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e2));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e6));
                    break;
                case (11): //0000 1011
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e1));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e4));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e8));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e8));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e2));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e6));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e6));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e8));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e7));
                    break;
                case (12): //0000 1100
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e8));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e4));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e2));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e2));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e8));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e7));
                    break;
                case (13): //0000 1101
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e8));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e5));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e6));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e6));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e8));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e3));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e3));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e6));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e2));
                    break;
                case (14): //0000 1110
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e1));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e4));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e8));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e8));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e1));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e6));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e6));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e8));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e7));
                    break;
                case (15): //0000 1111
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e5));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e6));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e8));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e8));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e6));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e7));
                    break;
                case (16): //0001 0000
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e12));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e9));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e5));
                    break;
                case (17): //0001 0001
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e9));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e1));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e12));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e12));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e9));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e4));
                    break;
                case (18): //0001 0010
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e5));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e9));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e12));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e1));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e2));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e6));
                    break;
                case (19): //0001 0011
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e5));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e9));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e12));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e4));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e2));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e5));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e5));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e6));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e2));
                    break;
                #endregion;
                #region 20-29
                case (20): //0001 0100
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e9));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e12));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e5));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e2));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e3));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e7));
                    break;
                case (21): //0001 0101
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e2));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e3));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e7));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e9));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e1));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e12));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e12));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e9));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e4));
                    break;
                case (22): //0001 0110
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e1));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e3));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e6));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e6));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e7));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e3));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e9));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e12));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e5));
                    break;
                case (23): //0001 0111
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e9));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e12));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e6));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e6));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e7));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e3));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e3));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e6));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e12));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e12));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e4));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e3));
                    break;
                case (24): //0001 1000
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e9));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e12));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e5));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e4));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e3));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e8));
                    break;
                case (25): //0001 1001
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e12));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e3));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e9));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e9));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e1));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e3));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e3));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e8));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e12));
                    break;
                case (26): //0001 1010
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e4));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e3));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e8));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e1));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e2));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e6));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e9));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e12));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e5));
                    break;
                case (27): //0001 1011
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e9));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e12));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e8));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e8));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e9));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e3));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e3));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e9));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e6));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e6));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e3));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e2));
                    break;
                case (28): //0001 1100
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e8));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e4));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e2));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e2));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e8));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e7));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e9));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e12));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e5));
                    break;
                case (29): //0001 1101
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e12));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e9));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e8));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e8));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e7));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e2));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e2));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e8));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e9));

                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e9));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e2));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(e1));
                    break;
                #endregion;
                #region 30-39
                case (30): //0001 1110
                    break;
                case (31): //0001 1111
                    break;
                case (32): //0010 0000
                    break;
                case (33): //0010 0001
                    break;
                case (34): //0010 0010
                    break;
                case (35): //0010 0011
                    break;
                case (36): //0010 0100
                    break;
                case (37): //0010 0101
                    break;
                case (38): //0010 0110
                    break;
                case (39): //0010 0111
                    break;
                #endregion;
                #region 40-49
                case (40):
                    break;
                case (41):
                    break;
                case (42):
                    break;
                case (43):
                    break;
                case (44):
                    break;
                case (45):
                    break;
                case (46):
                    break;
                case (47):
                    break;
                case (48):
                    break;
                case (49):
                    break;
                #endregion;
                #region 50-59
                case (50):
                    break;
                case (51):
                    break;
                case (52):
                    break;
                case (53):
                    break;
                case (54):
                    break;
                case (55):
                    break;
                case (56):
                    break;
                case (57):
                    break;
                case (58):
                    break;
                case (59):
                    break;
                #endregion;
                #region 60-69
                case (60):
                    break;
                case (61):
                    break;
                case (62):
                    break;
                case (63):
                    break;
                case (64):
                    break;
                case (65):
                    break;
                case (66):
                    break;
                case (67):
                    break;
                case (68):
                    break;
                case (69):
                    break;
                #endregion;
                #region 70-79
                case (70):
                    break;
                case (71):
                    break;
                case (72):
                    break;
                case (73):
                    break;
                case (74):
                    break;
                case (75):
                    break;
                case (76):
                    break;
                case (77):
                    break;
                case (78):
                    break;
                case (79):
                    break;
                #endregion;
                #region 80-89
                case (80):
                    break;
                case (81):
                    break;
                case (82):
                    break;
                case (83):
                    break;
                case (84):
                    break;
                case (85):
                    break;
                case (86):
                    break;
                case (87):
                    break;
                case (88):
                    break;
                case (89):
                    break;
                #endregion;
                #region 90-99
                case (90):
                    break;
                case (91):
                    break;
                case (92):
                    break;
                case (93):
                    break;
                case (94):
                    break;
                case (95):
                    break;
                case (96):
                    break;
                case (97):
                    break;
                case (98):
                    break;
                case (99):
                    break;
                #endregion;
                #endregion
                #region 100-199
                #region 100-109
                case (100):
                    break;
                case (101):
                    break;
                case (102):
                    break;
                case (103):
                    break;
                case (104):
                    break;
                case (105):
                    break;
                case (106):
                    break;
                case (107):
                    break;
                case (108):
                    break;
                case (109):
                    break;
                #endregion;
                #region 110-119
                case (110):
                    break;
                case (111):
                    break;
                case (112):
                    break;
                case (113):
                    break;
                case (114):
                    break;
                case (115):
                    break;
                case (116):
                    break;
                case (117):
                    break;
                case (118):
                    break;
                case (119):
                    break;
                #endregion;
                #region 120-129
                case (120):
                    break;
                case (121):
                    break;
                case (122):
                    break;
                case (123):
                    break;
                case (124):
                    break;
                case (125):
                    break;
                case (126):
                    break;
                case (127):
                    break;
                case (128):
                    break;
                case (129):
                    break;
                #endregion;
                #region 130-139
                case (130):
                    break;
                case (131):
                    break;
                case (132):
                    break;
                case (133):
                    break;
                case (134):
                    break;
                case (135):
                    break;
                case (136):
                    break;
                case (137):
                    break;
                case (138):
                    break;
                case (139):
                    break;
                #endregion;
                #region 140-149
                case (140):
                    break;
                case (141):
                    break;
                case (142):
                    break;
                case (143):
                    break;
                case (144):
                    break;
                case (145):
                    break;
                case (146):
                    break;
                case (147):
                    break;
                case (148):
                    break;
                case (149):
                    break;
                #endregion;
                #region 150-159
                case (150):
                    break;
                case (151):
                    break;
                case (152):
                    break;
                case (153):
                    break;
                case (154):
                    break;
                case (155):
                    break;
                case (156):
                    break;
                case (157):
                    break;
                case (158):
                    break;
                case (159):
                    break;
                #endregion;
                #region 160-169
                case (160):
                    break;
                case (161):
                    break;
                case (162):
                    break;
                case (163):
                    break;
                case (164):
                    break;
                case (165):
                    break;
                case (166):
                    break;
                case (167):
                    break;
                case (168):
                    break;
                case (169):
                    break;
                #endregion;
                #region 170-179
                case (170):
                    break;
                case (171):
                    break;
                case (172):
                    break;
                case (173):
                    break;
                case (174):
                    break;
                case (175):
                    break;
                case (176):
                    break;
                case (177):
                    break;
                case (178):
                    break;
                case (179):
                    break;
                #endregion;
                #region 180-189
                case (180):
                    break;
                case (181):
                    break;
                case (182):
                    break;
                case (183):
                    break;
                case (184):
                    break;
                case (185):
                    break;
                case (186):
                    break;
                case (187):
                    break;
                case (188):
                    break;
                case (189):
                    break;
                #endregion;
                #region 190-199
                case (190):
                    break;
                case (191):
                    break;
                case (192):
                    break;
                case (193):
                    break;
                case (194):
                    break;
                case (195):
                    break;
                case (196):
                    break;
                case (197):
                    break;
                case (198):
                    break;
                case (199):
                    break;
                #endregion;
                #endregion
                #region 200-255
                #region 200-209
                case (200):
                    break;
                case (201):
                    break;
                case (202):
                    break;
                case (203):
                    break;
                case (204):
                    break;
                case (205):
                    break;
                case (206):
                    break;
                case (207):
                    break;
                case (208):
                    break;
                case (209):
                    break;
                #endregion;
                #region 210-219
                case (210):
                    break;
                case (211):
                    break;
                case (212):
                    break;
                case (213):
                    break;
                case (214):
                    break;
                case (215):
                    break;
                case (216):
                    break;
                case (217):
                    break;
                case (218):
                    break;
                case (219):
                    break;
                #endregion;
                #region 220-229
                case (220):
                    break;
                case (221):
                    break;
                case (222):
                    break;
                case (223):
                    break;
                case (224):
                    break;
                case (225):
                    break;
                case (226):
                    break;
                case (227):
                    break;
                case (228):
                    break;
                case (229):
                    break;
                #endregion;
                #region 230-239
                case (230):
                    break;
                case (231):
                    break;
                case (232):
                    break;
                case (233):
                    break;
                case (234):
                    break;
                case (235):
                    break;
                case (236):
                    break;
                case (237):
                    break;
                case (238):
                    break;
                case (239):
                    break;
                #endregion;
                #region 240-249
                case (240):
                    break;
                case (241):
                    break;
                case (242):
                    break;
                case (243):
                    break;
                case (244):
                    break;
                case (245):
                    break;
                case (246):
                    break;
                case (247):
                    break;
                case (248):
                    break;
                case (249):
                    break;
                #endregion;
                #region 250-255
                case (250): //1111 1010
                    break;
                case (251): //1111 1011
                    break;
                case (252): //1111 1100
                    break;
                case (253): //1111 1101
                    break;
                case (254): //1111 1110
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(GetVertex(1, x, y, z)));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(GetVertex(5, x, y, z)));
                    mBuilder.AddTriangleVertex(mBuilder.CreatePosition(GetVertex(4, x, y, z)));
                    break;
                case (255): //1111 1111
                    //Do nothing.
                    break;
                #endregion
                #endregion
                default:
                    break;
            }
        }

        private Vector3 GetVertex(int vertex, int x, int y, int z)
        {
            switch (vertex)
            {
                case 1:
                    return new Vector3(2 * x + 1, 2 * y, 2 * z);
                case 2:
                    return new Vector3(2 * x + 2, 2 * y + 1, 2 * z);
                case 3:
                    return new Vector3(2 * x + 1, 2 * y + 2, 2 * z);
                case 4:
                    return new Vector3(2 * x, 2 * y + 1, 2 * z);
                case 5:
                    return new Vector3(2 * x, 2 * y, 2 * z + 1);
                case 6:
                    return new Vector3(2 * x + 2, 2 * y, 2 * z + 1);
                case 7:
                    return new Vector3(2 * x + 2, 2 * y + 2, 2 * z + 1);
                case 8:
                    return new Vector3(2 * x, 2 * y + 2, 2 * z + 1);
                case 9:
                    return new Vector3(2 * x + 1, 2 * y, 2 * z + 2);
                case 10:
                    return new Vector3(2 * x + 2, 2 * y + 1, 2 * z + 2);
                case 11:
                    return new Vector3(2 * x + 1, 2 * y + 2, 2 * z + 2);
                case 12:
                    return new Vector3(2 * x, 2 * y + 2, 2 * z + 2);
                default:
                    throw new Exception("Unknown vertex");
            }
        }

        private Tuple<byte, byte[]>[,] ProcessTexturedSlices(byte[,] slice1, byte[,] slice2)
        {
            byte index;
            Tuple<byte, byte[]>[,] cubes = new Tuple<byte, byte[]>[WORLD_WIDTH - 1, WORLD_HEIGHT - 1];
            byte v1, v2, v3, v4, v5, v6, v7, v8;
            byte[] type;


            for (int y = 0; y < WORLD_HEIGHT - 1; y++)
            {
                for (int x = 0; x < WORLD_WIDTH - 1; x++)
                {
                    type = new byte[8];
                    if (slice1 == null)
                    {
                        type[0] = 0;
                        v1 = 0;

                        type[1] = 0;
                        v2 = 0;

                        type[2] = 0;
                        v3 = 0;

                        type[3] = 0;
                        v4 = 0;

                        type[4] = slice2[x, y];
                        v5 = (type[4] != 0) ? (byte)1 : (byte)0;

                        type[5] = slice2[x + 1, y];
                        v6 = (type[5] != 0) ? (byte)1 : (byte)0;

                        type[6] = slice2[x + 1, y + 1];
                        v7 = (type[6] != 0) ? (byte)1 : (byte)0;

                        type[7] = slice2[x, y + 1];
                        v8 = (type[7] != 0) ? (byte)1 : (byte)0;
                    }
                    else if (slice2 == null)
                    {
                        type[0] = slice1[x, y];
                        v1 = (type[0] != 0) ? (byte)1 : (byte)0;

                        type[1] = slice1[x + 1, y];
                        v2 = (type[1] != 0) ? (byte)1 : (byte)0;

                        type[2] = slice1[x + 1, y + 1];
                        v3 = (type[2] != 0) ? (byte)1 : (byte)0;

                        type[3] = slice1[x, y + 1];
                        v4 = (type[3] != 0) ? (byte)1 : (byte)0;

                        type[4] = 0;
                        v5 = 0;

                        type[5] = 0;
                        v6 = 0;

                        type[6] = 0;
                        v7 = 0;

                        type[7] = 0;
                        v8 = 0;
                    }
                    else
                    {
                        type[0] = slice1[x, y];
                        v1 = (type[0] != 0) ? (byte)1 : (byte)0;

                        type[1] = slice1[x + 1, y];
                        v2 = (type[1] != 0) ? (byte)1 : (byte)0;

                        type[2] = slice1[x + 1, y + 1];
                        v3 = (type[2] != 0) ? (byte)1 : (byte)0;

                        type[3] = slice1[x, y + 1];
                        v4 = (type[3] != 0) ? (byte)1 : (byte)0;

                        type[4] = slice2[x, y];
                        v5 = (type[4] != 0) ? (byte)1 : (byte)0;

                        type[5] = slice2[x + 1, y];
                        v6 = (type[5] != 0) ? (byte)1 : (byte)0;

                        type[6] = slice2[x + 1, y + 1];
                        v7 = (type[6] != 0) ? (byte)1 : (byte)0;

                        type[7] = slice2[x, y + 1];
                        v8 = (type[7] != 0) ? (byte)1 : (byte)0;
                    }
                    index = (byte)((v1) | (v2 << 1) | (v3 << 2) | (v4 << 3) | (v5 << 4) | (v6 << 5) | (v7 << 6) | (v8 << 7));
                    cubes[x, y] = new Tuple<byte, byte[]>(index, type);
                }
            }

            return cubes;
        }

        private void AddToTexturedMesh(Tuple<byte, byte[]>[,] tIndex, int z)
        {

            throw new NotImplementedException();
        }
    }
        #endregion

    class BlockData
    {
        private int _x, _y, _z;
        private byte _type;

        public int x
        {
            get
            {
                return _x;
            }
        }

        public int y
        {
            get
            {
                return _y;
            }
        }

        public int z
        {
            get
            {
                return _z;
            }
        }

        public byte Type
        {
            get
            {
                return _type;
            }
        }
    }
}