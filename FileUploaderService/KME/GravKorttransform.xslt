<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
                exclude-result-prefixes="msxsl"
>
    <xsl:output method="html" indent="yes" encoding="UTF-8"/>
 
  <xsl:param name="iso-string" select="'&#161;Hola, C&#233;sar!'"/>

  <!-- Characters we'll support.
       We could add control chars 0-31 and 127-159, but we won't. -->
  <xsl:variable name="ascii"> !"#$%&amp;'()*+,-./0123456789:;&lt;=&gt;?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~</xsl:variable>
  <xsl:variable name="latin1">&#160;&#161;&#162;&#163;&#164;&#165;&#166;&#167;&#168;&#169;&#170;&#171;&#172;&#173;&#174;&#175;&#176;&#177;&#178;&#179;&#180;&#181;&#182;&#183;&#184;&#185;&#186;&#187;&#188;&#189;&#190;&#191;&#192;&#193;&#194;&#195;&#196;&#197;&#198;&#199;&#200;&#201;&#202;&#203;&#204;&#205;&#206;&#207;&#208;&#209;&#210;&#211;&#212;&#213;&#214;&#215;&#216;&#217;&#218;&#219;&#220;&#221;&#222;&#223;&#224;&#225;&#226;&#227;&#228;&#229;&#230;&#231;&#232;&#233;&#234;&#235;&#236;&#237;&#238;&#239;&#240;&#241;&#242;&#243;&#244;&#245;&#246;&#247;&#248;&#249;&#250;&#251;&#252;&#253;&#254;&#255;</xsl:variable>

  <!-- Characters that usually don't need to be escaped -->
  <xsl:variable name="safe">!'()*-.0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz~</xsl:variable>

  <xsl:variable name="hex" >0123456789ABCDEF</xsl:variable>



  <xsl:template match="/StartingListLag">

      <xsl:variable name="Stevnenavn">
        <xsl:call-template name="url-encode">
          <xsl:with-param name="str" select="StevneNavn" />
        </xsl:call-template>
      </xsl:variable>
      <html>
        <head>
          <link rel="stylesheet" type="text/css" href="leon.css" />
          <title>
            <xsl:value-of select="StevneNavn"/>
          </title>
        </head>
        <body>
          <p class="MsoNormal">
            <xsl:value-of select="concat('Lag:',LagNr,'  Skytetid ',substring-before(StartTime,'T'),'  ',substring-after(StartTime,'T'))"/></p>
          <table border="0" width="100%">
            <tr>
              <td width="5%" align="center">
                <b>Sk.</b>
              </td>
              <td width="15%" align="left">
                <b>Navn</b>
              </td>
              <td width="12%" align="left">
                <b>Skytterlag</b>
              </td>
              <td width="6%" align="center">
                <b>Kl.</b>
              </td>
            </tr>
            <xsl:for-each select="Skiver/StartingListSkive">
              <tr>
                <td width="5%" align="center">
                  <b>
                    <xsl:value-of select="SkiveNr"/>
                  </b>
                </td>
                <xsl:variable name="hrefName">
                  <xsl:value-of select="concat('stevner/',$Stevnenavn,'/',BackUpBitMapFileName)"/>
                </xsl:variable>
                <td width="15%" align="left">
                  <xsl:choose>
                    <xsl:when test="string(BackUpBitMapFileName)!=''">
                      <b href='{$hrefName}'>
                        <xsl:value-of select="SkytterNavn"/>
                      </b>
                    </xsl:when>
                    <xsl:otherwise>
                      <b>
                        <xsl:value-of select="SkytterNavn"/>
                      </b>
                    </xsl:otherwise>
                  </xsl:choose>
                 
                </td>
                <td width="12%" align="left">
                  <b>
                    <xsl:value-of select="SkytterLag"/>
                  </b>
                </td>
                <td width="6%" align="center">
                  <b>
                    <xsl:value-of select="Klasse"/>
                  </b>
                </td>
              </tr>
            
            </xsl:for-each>
           </table>
          <h3 align="center">Bodø Østre skytterlag</h3>
          <h4 align="center"> Leon - Kongsberg Target Systems AS - www.kongsberg-ts.no</h4>
          <h4 align="center"> Oppdatert 06.01.2015 15:19:04</h4>
        </body>

      </html>  
    </xsl:template>

  <xsl:template name="url-encode">
    <xsl:param name="str"/>
    <xsl:if test="$str">
      <xsl:variable name="first-char" select="substring($str,1,1)"/>
      <xsl:choose>
        <xsl:when test="contains($safe,$first-char)">
          <xsl:value-of select="$first-char"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:variable name="codepoint">
            <xsl:choose>
              <xsl:when test="contains($ascii,$first-char)">
                <xsl:value-of select="string-length(substring-before($ascii,$first-char)) + 32"/>
              </xsl:when>
              <xsl:when test="contains($latin1,$first-char)">
                <xsl:value-of select="string-length(substring-before($latin1,$first-char)) + 160"/>
              </xsl:when>
              <xsl:otherwise>
                <xsl:message terminate="no">Warning: string contains a character that is out of range! Substituting "?".</xsl:message>
                <xsl:text>63</xsl:text>
              </xsl:otherwise>
            </xsl:choose>
          </xsl:variable>
          <xsl:variable name="hex-digit1" select="substring($hex,floor($codepoint div 16) + 1,1)"/>
          <xsl:variable name="hex-digit2" select="substring($hex,$codepoint mod 16 + 1,1)"/>
          <xsl:value-of select="concat('%',$hex-digit1,$hex-digit2)"/>
        </xsl:otherwise>
      </xsl:choose>
      <xsl:if test="string-length($str) &gt; 1">
        <xsl:call-template name="url-encode">
          <xsl:with-param name="str" select="substring($str,2)"/>
        </xsl:call-template>
      </xsl:if>
    </xsl:if>
  </xsl:template>


</xsl:stylesheet>
