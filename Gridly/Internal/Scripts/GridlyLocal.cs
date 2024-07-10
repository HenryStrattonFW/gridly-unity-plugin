using UnityEngine;
using System;
using Gridly.Internal;

namespace Gridly
{
    public enum Languages
    {
        enUS,
        arSA,
        frFR,
        zhCN,
        zhTW,
        deDE,
        itIT,
        jaJP,
        koKR,
        plPL,
        ptBR,
        ruRU,
        esES,
        esMX,
        caES,
        bnBD,
        bgBG,
        zhHK,
        afZA,
        arAE,
        arBH,
        arDZ,
        arEG,
        arIQ,
        arJO,
        arKW,
        arLB,
        arLY,
        arMA,
        arOM,
        arQA,
        arSY,
        arTN,
        arYE,
        azAZ,
        beBY,
        bsBA,
        csCZ,
        cyGB,
        daDK,
        deAT,
        deCH,
        deLI,
        deLU,
        dvMV,
        elGR,
        enAU,
        enBZ,
        enCA,
        enGB,
        enIE,
        enJM,
        enNZ,
        enPH,
        enTT,
        enZA,
        enZW,
        esAR,
        esBO,
        esCL,
        esCO,
        esCR,
        esDO,
        esEC,
        esGT,
        esHN,
        esNI,
        esPA,
        esPE,
        esPR,
        esPY,
        esSV,
        esUY,
        esVE,
        etEE,
        euES,
        faIR,
        fiFI,
        foFO,
        frBE,
        frCA,
        frCH,
        frLU,
        frMC,
        glES,
        guIN,
        heIL,
        hiIN,
        hrBA,
        hrHR,
        huHU,
        hyAM,
        idID,
        isIS,
        itCH,
        kaGE,
        kkKZ,
        knIN,
        kyKG,
        ltLT,
        lvLV,
        miNZ,
        mkMK,
        mnMN,
        mrIN,
        msBN,
        msMY,
        mtMT,
        nbNO,
        nlBE,
        nlNL,
        nnNO,
        nsZA,
        paIN,
        psAR,
        ptPT,
        quBO,
        quEC,
        quPE,
        roRO,
        saIN,
        seFI,
        seNO,
        seSE,
        skSK,
        siSI,
        sqAL,
        srBA,
        svFI,
        svSE,
        swKE,
        taIN,
        teIN,
        thTH,
        tlPH,
        tnZA,
        trTR,
        ttRU,
        ukUA,
        urPK,
        uzUZ,
        viVN,
        xhZA,
        zhMO,
        zhSG,
        zuZA,
    }

    public static class GridlyLocal
    {
        /// <summary>
        /// Attempts to get the localized value of a record based on the current target langauge.
        /// </summary>
        /// <param name="gridName">ID of the grid to search in.</param>
        /// <param name="recordID">ID of the record to retrieve.</param>
        /// <returns>Translated text if found. Empty string otherwise.</returns>
        public static string GetStringData(string gridName, string recordID)
        {
            try
            {
                GridlyLogging.Log($"gridId: {gridName} recordID: {recordID}");
                var grid = Project.Singleton.GetGridByName(gridName);
                var record = grid.records.Find(x => x.recordID == recordID);

                foreach (var column in record.columns)
                {
                    try
                    {
                        if (column.columnID == Project.Singleton.TargetLanguage.languagesSupport.ToString())
                        {
                            return column.text;
                        }
                    }
                    catch // try to return another language if we fail to find target language
                    {
                        GridlyLogging.Log($"cant find: {recordID} | code:{Project.Singleton.TargetLanguage.languagesSupport}");
                        foreach(var lang in Project.Singleton.langSupports)
                        {
                            if (column.columnID == lang.languagesSupport.ToString())
                            {
                                return column.text;
                            }
                        }
                    }
                }

            }
            catch(Exception ex)
            {
                GridlyLogging.Log($"Path does not exist. Please make sure you entered the correct path format, and added data{ex.Message}");
            }

            return "";
        }
    }
}
