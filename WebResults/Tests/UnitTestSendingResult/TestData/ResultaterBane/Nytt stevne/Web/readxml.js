var resultnum;
var resultname;
var resulttotsum;
var resultinnerten;
var resultnote;
var resultclub;
var resultclass;
var resultclasssub;
var resultcategory;
var NumOfSeries;
var AllSeriesColNames = new Array();
var AllSeriesID = new Array();
var AllSeriesType = new Array();
$.extend({
    ParseLeonXML: function(xmlFileName) {
        var header;
        var teamnum;
        var teamname;
        var teamtotsum;
        var teaminnerten;
        var teamnote;
        var TableID;
        var Output;
        var TeamOutput;
        var TeamColumnNo;
        var TeamEnabled;
        var TeamNumTxt;
        var TeamNameTxt;
        var TeamTotSumTxt;
        var TeamInnerTenTxt;
        var TeamNoteTxt;
        var TeamNumCol = -1;
        var TeamNameCol = -1;
        var TeamTotSumCol = -1;
        var TeamInnerTenCol = -1;
        var TeamNoteCol = -1;
        var xmlFile = xmlFileName + '.xml';
        $.ajax({
            url: xmlFile,
            dataType: ($.browser.msie) ? "text" : "xml",
            cache: false,
            success: function(data) {
                var xml;
                if (typeof data == "string") {
                    xml = new ActiveXObject("Microsoft.XMLDOM");
                    xml.async = false;
                    xml.loadXML(data)
                } else {
                    xml = data
                }
                header = $(xml).find("header").attr("name");
                header = header + ' - ' + $(xml).find("header").attr("group_name");
                teamnum = $(xml).find("teamnum").attr("name");
                teamname = $(xml).find("teamname").attr("name");
                teamtotsum = $(xml).find("teamtotsum").attr("name");
                teaminnerten = $(xml).find("teaminnerten").attr("name");
                teamnote = $(xml).find("teamote").attr("name");
                resultnum = $(xml).find("resultnum").attr("name");
                resultname = $(xml).find("resultname").attr("name");
                resultclub = $(xml).find("resultclub").attr("name");
                resultclass = $(xml).find("resultclass").attr("name");
                resultclasssub = $(xml).find("resultclasssub").attr("name");
                resultcategory = $(xml).find("resultcategory").attr("name");
                resulttotsum = $(xml).find("resulttotsum").attr("name");
                resultinnerten = $(xml).find("resultinnerten").attr("name");
                resultnote = $(xml).find("resultnote").attr("name");
                ColCount = 0;
                if (resultnum != undefined) {
                    if (teamnum != undefined) TeamNumCol = ColCount;
                    ColCount++
                }
                if (resultname != undefined) {
                    if (teamname != undefined) TeamNameCol = ColCount;
                    ColCount++
                }
                if (resultclub != undefined) ColCount++;
                if (resultclass != undefined) ColCount++;
                if (resultclasssub != undefined) ColCount++;
                if (resultcategory != undefined) ColCount++;
                NumOfSeries = 0;
                $(xml).find('column').each(function() {
                    $(this).find('series').each(function() {
                        AllSeriesID[NumOfSeries] = $(this).attr("id");
                        AllSeriesColNames[NumOfSeries] = $(this).attr("name");
                        AllSeriesType[NumOfSeries] = $(this).attr("type");
                        if ($(this).attr("type") == "series_sum") {
                            ColCount++
                        }
                        if ($(this).attr("type") == "series_shot") {
                            ColCount++
                        }
                        if ($(this).attr("type") == "series_shot_sum") {
                            ColCount += 2
                        }
                        if ($(this).attr("type") == "subtot") {
                            ColCount++
                        }
                        NumOfSeries++
                    })
                });
                if (resulttotsum != undefined) {
                    if (teamtotsum != undefined) TeamTotSumCol = ColCount;
                    ColCount++
                }
                if (resultinnerten != undefined) {
                    if (teaminnerten != undefined) TeamInnerTenCol = ColCount;
                    ColCount++
                }
                if (resultnote != undefined) {
                    if (teamnote != undefined) TeamNoteCol = ColCount;
                    ColCount++
                }
                TeamEnabled = 0;
                if (teamnum != undefined || teamname != undefined || teamtotsum != undefined || teaminnerten != undefined || teamnote != undefined) {
                    TeamEnabled = 1
                }
                Output = '<p id="reportname">' + header + '</p>';
                $(xml).find('data').each(function() {
                    if (TeamEnabled == 0) {
                        Output += '<table id="resulttable">';
                        Output += ParseResultHeader();
                        Output += ParseResult(this);
                        Output += '</table>'
                    } else {
                        Output += '<table id="resulttable">';
                        Output += ParseResultHeader();
                        $(this).find('team').each(function() {
                            TeamOutput = '';
                            if (teamnum != undefined) TeamNumTxt = $(this).attr("num");
                            if (teamname != undefined) TeamNameTxt = $(this).attr("name");
                            if (teamtotsum != undefined) TeamTotSumTxt = $(this).attr("totsum");
                            if (teaminnerten != undefined) TeamInnerTenTxt = $(this).attr("innerten");
                            if (teamnote != undefined) TeamNoteTxt = $(this).attr("innerten");
                            TeamOutput += '<tr>';
                            for (i = 0; i < ColCount; i++) {
                                if (TeamNumCol == i) TeamOutput += '<th id="teamnum" >' + TeamNumTxt + '</th>';
                                else if (TeamNameCol == i) TeamOutput += '<th id="teamname" >' + TeamNameTxt + '</th>';
                                else if (TeamTotSumCol == i) TeamOutput += '<th id="teamtotsum" >' + TeamTotSumTxt + '</th>';
                                else if (TeamInnerTenCol == i) TeamOutput += '<th id="teaminnerten" >' + TeamInnerTenTxt + '</th>';
                                else if (TeamNoteCol == i) TeamOutput += '<th id="teamnote" >' + TeamNoteTxt + '</th>';
                                else TeamOutput += '<th ></th>'
                            }
                            TeamOutput += '</tr>';
                            TeamOutput += ParseResult(this);
                            Output += TeamOutput
                        });
                        Output += '</table>'
                    }
                });
                Output += '<br>';
                $("#loader").empty();
                $("#ContentArea").html(Output);
                $("table tr:nth-child(even)").addClass("even")
            }
        })
    }
});

function ParseResult(xml) {
    var ResultRowOutput;
    var ResultTableOutput;
    var ResultNumTxt;
    var ResultNameTxt;
    var ResultClubTxt;
    var ResultClassTxt;
    var ResultClassSubTxt;
    var ResultCategoryTxt;
    var ResultTotSumTxt;
    var ResultInnerTenTxt;
    var ResultNoteTxt;
    var CurrentSeries;
    var ShowSum;
    var ShowShot;
    var ShotTxt;
    var SeriesSumTxt;
    var png;
    ResultTableOutput = '';
    $(xml).find("result").each(function() {
        ResultRowOutput = '';
        if (resultnum != undefined) {
            ResultNumTxt = $(this).attr("num");
            ResultRowOutput += '<td id="resultnum" >' + ResultNumTxt + '</td>'
        }
        if (resultname != undefined) {
            ResultNameTxt = $(this).attr("name");
            png = $(this).attr("ref");
            if (png)
            ResultRowOutput += '<td id="resultname" ><a href="'+ png + '" target="_blank" onmouseover="style.textDecoration=\'underline\'" onmouseout="style.textDecoration=\'none\'" style="color:#0066cc;">' + ResultNameTxt + '</td>'
             else
            ResultRowOutput += '<td id="resultname" >' + ResultNameTxt + '</td>'
        }
        if (resultclub != undefined) {
            ResultClubTxt = $(this).attr("club");
            ResultRowOutput += '<td id="resultclub" >' + ResultClubTxt + '</td>'
        }
        if (resultclass != undefined) {
            ResultClassTxt = $(this).attr("class");
            ResultRowOutput += '<td id="resultclass" >' + ResultClassTxt + '</td>'
        }
        if (resultclasssub != undefined) {
            ResultClassSubTxt = $(this).attr("classsub");
            ResultRowOutput += '<td id="resultclasssub" >' + ResultClassSubTxt + '</td>'
        }
        if (resultcategory != undefined) {
            ResultCategoryTxt = $(this).attr("category");
            ResultRowOutput += '<td id="resultcategory" >' + ResultCategoryTxt + '</td>'
        }
        CurrentSeries = 0;
        $(this).find("series").each(function() {
            if (AllSeriesID[CurrentSeries] == $(this).attr("id")) {
                ShowSum = false;
                ShowShot = false;
                if (AllSeriesType[CurrentSeries] == "series_sum") {
                    ShowSum = true
                }
                if (AllSeriesType[CurrentSeries] == "series_shot") {
                    ShowShot = true
                }
                if (AllSeriesType[CurrentSeries] == "series_shot_sum") {
                    ShowSum = true;
                    ShowShot = true
                }
                if (AllSeriesType[CurrentSeries] == "subtot") {
                    ShowSum = true
                }
                ShotTxt = '';
                SeriesSumTxt = '';
                if (ShowShot == true) {
                    $(this).find("shot").each(function() {
                        if ($(this).attr("val").indexOf(".") > 0) ShotTxt += $(this).attr("val") + " ";
                        else ShotTxt += $(this).attr("val")
                    });
                    ResultRowOutput += '<td id="seriesshot" >' + ShotTxt + '</td>'
                }
                if (ShowSum == true) {
                    SeriesSumTxt = $(this).attr("sum");
                    ResultRowOutput += '<td id="seriessum" >' + SeriesSumTxt + '</td>'
                }
                CurrentSeries++
            }
        });
        if (resulttotsum != undefined) {
            ResultTotSumTxt = $(this).attr("totsum");
            ResultRowOutput += '<td id="resulttotsum" >' + ResultTotSumTxt + '</td>'
        }
        if (resultinnerten != undefined) {
            ResultInnerTenTxt = $(this).attr("innerten");
            ResultRowOutput += '<td id="resultinnerten" >' + ResultInnerTenTxt + '</td>'
        }
        if (resultnote != undefined) {
            ResultNoteTxt = $(this).attr("note");
            ResultRowOutput += '<td id="resultnote" >' + ResultNoteTxt + '</td>'
        }
        ResultTableOutput += '<tr>' + ResultRowOutput + '</tr>'
    });
    return ResultTableOutput
}

function ParseResultHeader() {
    var ResultHeaderTableOutput;
    var i;
    ResultHeaderTableOutput = '';
    if (resultnum != undefined) {
        ResultHeaderTableOutput += '<th id="header_resultnum" >' + resultnum + '</th>'
    }
    if (resultname != undefined) {
        ResultHeaderTableOutput += '<th id="header_resultname" >' + resultname + '</th>'
    }
    if (resultclub != undefined) {
        ResultHeaderTableOutput += '<th id="header_resultclub" >' + resultclub + '</th>'
    }
    if (resultclass != undefined) {
        ResultHeaderTableOutput += '<th id="header_resultclass" >' + resultclass + '</th>'
    }
    if (resultclasssub != undefined) {
        ResultHeaderTableOutput += '<th id="header_resultclasssub" >' + resultclasssub + '</th>'
    }
    if (resultcategory != undefined) {
        ResultHeaderTableOutput += '<th id="header_resultcategory" >' + resultcategory + '</th>'
    }
    for (i = 0; i < NumOfSeries; i++) {
        if (AllSeriesType[i] == "series_shot_sum") {
            ResultHeaderTableOutput += '<th colspan="2" id="header_' + AllSeriesType[i] + '" >' + AllSeriesColNames[i] + '</th>'
        } else {
            ResultHeaderTableOutput += '<th id="header_' + AllSeriesType[i] + '" >' + AllSeriesColNames[i] + '</th>'
        }
    }
    if (resulttotsum != undefined) {
        ResultHeaderTableOutput += '<th id="header_resulttotsum" >' + resulttotsum + '</th>'
    }
    if (resultinnerten != undefined) {
        ResultHeaderTableOutput += '<th id="header_resultinnerten" >' + resultinnerten + '</th>'
    }
    if (resultnote != undefined) {
        ResultHeaderTableOutput += '<th id="header_resultnote" >' + resultnote + '</th>'
    }
    return ResultHeaderTableOutput
}