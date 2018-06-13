var resultnum;
var resultname;
var resultpngref;
var resulttotsum;
var resultinnerten;
var resultnote;
var resultclub;
var resultclass;
var resultclasssub;
var resultcategory;
var NumOfSeries;
var AllSeriesColNames=new Array();
var AllSeriesID=new Array();
var AllSeriesType=new Array();
$.extend({
    ParseLeonXML: function (c) {
		var d;
		var e;
		var f;
		var g;
		var h;
		var j;
		var k;
		var l;
		var m;
		var n;
		var o;
		var p;
		var q;
		var r;
		var s;
		var t;
		var u=-1;
		var v=-1;
		var w=-1;
		var x=-1;
		var y=-1;
		var z=c+'.xml';
		$.ajax({
			url: z, dataType: ($.browser.msie) ? "text" : "xml", cache: false, success: function (a) {
				var b;
				if(typeof a=="string"){
					b=new ActiveXObject("Microsoft.XMLDOM");
					b.async=false;
					b.loadXML(a)
				} else {
					b=a
				}
				d=$(b).find("header").attr("name");
				e=$(b).find("teamnum").attr("name");
				f=$(b).find("teamname").attr("name");
				g=$(b).find("teamtotsum").attr("name");
				h=$(b).find("teaminnerten").attr("name");
				j=$(b).find("teamote").attr("name");
				resultnum=$(b).find("resultnum").attr("name");
				resultname = $(b).find("resultname").attr("name");
				resultclub=$(b).find("resultclub").attr("name");
				resultclass=$(b).find("resultclass").attr("name");
				resultclasssub=$(b).find("resultclasssub").attr("name");
				resultcategory=$(b).find("resultcategory").attr("name");
				resulttotsum=$(b).find("resulttotsum").attr("name");
				resultinnerten=$(b).find("resultinnerten").attr("name");
				resultnote=$(b).find("resultnote").attr("name");
				ColCount=0;
				if(resultnum!=undefined) {
					if(e!=undefined)
						u=ColCount;
						
					ColCount++
				}
				if(resultname!=undefined) {
					if(f!=undefined)
						v=ColCount;
						
					ColCount++
				}
				if(resultclub!=undefined)
					ColCount++;
				if(resultclass!=undefined)
					ColCount++;
				if(resultclasssub!=undefined)
					ColCount++;
				if(resultcategory!=undefined)
					ColCount++;
				
				NumOfSeries=0;
				$(b).find('column').each(function() {
					$(this).find('series').each(function() {
						AllSeriesID[NumOfSeries]=$(this).attr("id");
						AllSeriesColNames[NumOfSeries]=$(this).attr("name");
						AllSeriesType[NumOfSeries]=$(this).attr("type");
						if($(this).attr("type")=="series_sum") {
							ColCount++
						}
						if($(this).attr("type")=="series_shot") {
							ColCount++
						}
						if($(this).attr("type")=="series_shot_sum") {
							ColCount+=2
						}
						if($(this).attr("type")=="subtot") {
							ColCount++
						}
						
						NumOfSeries++
					})
				});
				if(resulttotsum!=undefined) {
					if(g!=undefined)
						w=ColCount;
					
					ColCount++
				}
				if(resultinnerten!=undefined) {
					if(h!=undefined)
						x=ColCount;
						
					ColCount++
				}
				if(resultnote!=undefined) {
					if(j!=undefined)
						y=ColCount;
						
					ColCount++
				}
				
				o=0;
				
				if(e!=undefined||f!=undefined||g!=undefined||h!=undefined||j!=undefined) {
					o=1
				}
				l='<p id="reportname">'+d+'</p>';
				$(b).find('data').each(function() {
					if(o==0) {
						l+='<table id="resulttable">';
						l += ParseResultHeader();
						l += ParseResult(this, d);/*individuelt*/
						l+='</table>'
					} else {
						l+='<table id="resulttable">';
						l+=ParseResultHeader();
						$(this).find('team').each(function() {
							m='';
							if(e!=undefined)
								p=$(this).attr("num");
								
							if (f != undefined)
								q = $(this).attr("name");
								
							if(g!=undefined)
								r=$(this).attr("totsum");
								
							if(h!=undefined)
								s=$(this).attr("innerten");
								
							if(j!=undefined)
								t=$(this).attr("innerten");
							
							m+='<tr>';
							for(i=0;i<ColCount;i++) {
								if(u==i)
									m+='<th id="teamnum" >'+p+'</th>';
								else if (v == i)
									m += '<th id="teamname" >' + q + '</th>';
								else if (w == i)
									m += '<th id="teamtotsum" >' + r + '</th>';
								else if (x == i)
									m += '<th id="teaminnerten" >' + s + '</th>';
								else if (y == i)
									m += '<th id="teamnote" >' + t + '</th>';
								else
									m += '<th ></th>'
							}
							m+='</tr>';
							m += ParseResult(this, d); /*lagskyting*/
							l+=m
						});
						l+='</table>'
					}
				});
				l+='<br>';
				$("#loader").empty();
				$("#ContentArea").html(l);
				$("table tr:nth-child(even)").addClass("even")
			}
		})
	}
});

function ParseResult(a, r) {
	var b;
	var c;
	var d;
	var e;
	var f;
	var g;
	var h;
	var i;
	var j;
	var k;
	var l;
	var m;
	var n;
	var o;
	var p;
	var q;
	var png;
	c = '';
	var s = r.substring(0, 3);
	$(a).find("result").each(function() {
		b='';
		if (resultnum != undefined) {
            
		    d = $(this).attr("num"); /* var d er skivenummer */
			b+='<td id="resultnum" >'+d+'</td>'
		} if (resultname != undefined) {
            /*
                OPPRETT LINK HER
            */
		    e = $(this).attr("name");
		    png = $(this).attr("ref");
		    if (png)
               b += '<td id="resultname" ><a href="'+ png + '" target="_blank" onmouseover="style.textDecoration=\'underline\'" onmouseout="style.textDecoration=\'none\'" style="color:#0066cc;">' + e + '</a></td>'
		    else
                b += '<td id="resultname" >' + e + '</td>'
		} if (resultclub != undefined) {
			f=$(this).attr("club");
			b+='<td id="resultclub" >'+f+'</td>'
		}if(resultclass!=undefined) {
			g=$(this).attr("class");
			b+='<td id="resultclass" >'+g+'</td>'
		}if(resultclasssub!=undefined) {
			h=$(this).attr("classsub");
			b+='<td id="resultclasssub" >'+h+'</td>'
		}if(resultcategory!=undefined) {
			i=$(this).attr("category");
			b+='<td id="resultcategory" >'+i+'</td>'
		}
		m=0;
		$(this).find("series").each(function () {
		    png = $(this).attr("ref");
			if(AllSeriesID[m]==$(this).attr("id")) {
				n=false;
				o=false;
				if(AllSeriesType[m]=="series_sum") {
					n=true
				}if(AllSeriesType[m]=="series_shot") {
					o=true
				}if(AllSeriesType[m]=="series_shot_sum") {
					n=true;
					o=true
				}if(AllSeriesType[m]=="subtot") {
					n=true
				}
				p='';
				q='';
				if(o==true) {
					$(this).find("shot").each(function() {
						p+=$(this).attr("val")
					});
					b+='<td id="seriesshot" >'+p+'</td>'
				}if(n==true) {
					q=$(this).attr("sum");
					//b += '<td id="seriessum" >' + q + '</td>'
					if (png)
					    b += '<td id="seriessum" ><a href="' + png + '" target="_blank" onmouseover="style.textDecoration=\'underline\'" onmouseout="style.textDecoration=\'none\'" style="color:#0066cc;">' + q + '</a></td>'
					else
					    b += '<td id="seriessum" >' + q + '</td>'
				}m++
			}
		});
		if(resulttotsum!=undefined) {
			j=$(this).attr("totsum");
			b+='<td id="resulttotsum" >'+j+'</td>'
		}if(resultinnerten!=undefined) {
			k=$(this).attr("innerten");
			b+='<td id="resultinnerten" >'+k+'</td>'
		}if(resultnote!=undefined) {
			l=$(this).attr("note");
			b+='<td id="resultnote" >'+l+'</td>'
		}
		c+='<tr>'+b+'</tr>'
	});
	return c
}

function ParseResultHeader() {
	var a;
	var i;
	a='';
	if(resultnum!=undefined) {
		a+='<th id="header_resultnum" >'+resultnum+'</th>'
	}if(resultname!=undefined) {
		a+='<th id="header_resultname" >'+resultname+'</th>'
	}if(resultclub!=undefined) {
		a+='<th id="header_resultclub" >'+resultclub+'</th>'
	}if(resultclass!=undefined) {
		a+='<th id="header_resultclass" >'+resultclass+'</th>'
	}if(resultclasssub!=undefined) {
		a+='<th id="header_resultclasssub" >'+resultclasssub+'</th>'
	}if(resultcategory!=undefined) {
		a+='<th id="header_resultcategory" >'+resultcategory+'</th>'
	}
	
	for(i=0;i<NumOfSeries;i++) {
		if(AllSeriesType[i]=="series_shot_sum") {
			a+='<th colspan="2" id="header_'+AllSeriesType[i]+'" >'+AllSeriesColNames[i]+'</th>'
		} else {
			a+='<th id="header_'+AllSeriesType[i]+'" >'+AllSeriesColNames[i]+'</th>'
		}
	}
	if(resulttotsum!=undefined) {
		a+='<th id="header_resulttotsum" >'+resulttotsum+'</th>'
	}if(resultinnerten!=undefined) {
		a+='<th id="header_resultinnerten" >'+resultinnerten+'</th>'
	}if(resultnote!=undefined) {
		a+='<th id="header_resultnote" >'+resultnote+'</th>'
	}
	
	return a
}