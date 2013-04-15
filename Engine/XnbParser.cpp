#include "stdafx.h"
#include "ContentReader.h"
#include "XnbParser.h"

/* INSTRUCTIONS *********************************************************

To load data from a .xnb model:
------
#include "XnbModelData.h"
...
XnbModelData data("C:\\block.xnb");
vector<uint8_t> blockVertexData = data.vertexData;
------

To load data from a .xnb texture2d (currently only loads mip0):
------
#include "XnbTexture2dData.h"
...
XnbTexture2dData data(C:\\CatTexture.xnb);
vector<uint8_t> mip = data.mip0;
------

Tell me if you need me to add extra fields (like mip1, mip2, etc.) to the data classes (Samuel Fike sfike@ksu.edu)

*/

ContentReader XnbParser::parse(char* fileName)
{
    // Open the file.
    FILE* file;

    if (fopen_s(&file, fileName, "rb") != 0)
    {
        printf("Error: can't open '%s'.\n", fileName);
        //return ;
    }
	
    // Instantate the XNB reader.
    TypeReaderManager typeReaderManager;

    typeReaderManager.RegisterStandardTypes();

    ContentReader reader(file, &typeReaderManager);

    // Parse the XNB data.
	try
    {
        reader.ReadXnb();
    }
    catch (exception& e)
    {
        printf("Error: %s\n", e.what());
    }

    fclose(file);

    return reader;
}
