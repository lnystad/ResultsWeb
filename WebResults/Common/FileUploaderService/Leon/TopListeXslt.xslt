<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
      xmlns:msxsl="urn:schemas-microsoft-com:xslt"
      xmlns:utils="urn:myExtension"
     exclude-result-prefixes="msxsl utils">
  <xsl:output method="xml" indent="yes"/>

  <msxsl:script implements-prefix="utils" language="C#">
    <![CDATA[
      public string TuUpper(string stringValue)
      {
        string result = String.Empty;

        if(!String.IsNullOrEmpty(stringValue))
        {
          result = stringValue.ToUpper().Trim(); 
        }

        return result;
      }
    ]]>
  </msxsl:script>


  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="/">
    <xsl:apply-templates select="/Merged/report" />
  </xsl:template>

  <xsl:template match="result">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()"/>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="result/@ref">

  </xsl:template>

  <xsl:template match="result[@ref]">

    <xsl:variable name="LagskytingRapport">
      <xsl:choose>
        <xsl:when test="/Merged/report/header/@name='Ungdom'">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:when test="/Merged/report/header/@name='Ungdom individuell'">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:when test="/Merged/report/header/@name='Veteran'">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:when test="/Merged/report/header/@name='Veteran individuell'">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:when test="/Merged/report/header/@name='Senior'">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:when test="/Merged/report/header/@name='Senior individuell'">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="boolean(0)"/>
        </xsl:otherwise>
      </xsl:choose>


    </xsl:variable>


    <xsl:variable name="ClassValue">
      <xsl:value-of select="@class"/>
    </xsl:variable>
    <xsl:variable name="UseClass">
      <xsl:choose>
        <xsl:when test="string($ClassValue)!=''">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="boolean(0)"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>

    <xsl:variable name="CurrentSkytter">
      <xsl:value-of select="concat(utils:TuUpper(@name),utils:TuUpper(@club),utils:TuUpper(@class))"/>
    </xsl:variable>

    <xsl:variable name="FoundSkyttere">
      <Hits>
        <xsl:for-each select="/Merged/ArrayOfStartingListSkive" >
          <xsl:for-each select="StartingListSkive" >
            <xsl:variable name="CurrentRowSkytter">
              <xsl:choose>
                <xsl:when test="string($UseClass)='true'">
                  <xsl:value-of select="concat(utils:TuUpper(@name),utils:TuUpper(@club),utils:TuUpper(@class))"/>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="concat(utils:TuUpper(@name),utils:TuUpper(@club))"/>
                </xsl:otherwise>
              </xsl:choose>

            </xsl:variable>

            <xsl:if test="$CurrentSkytter = $CurrentRowSkytter">
              <Hit>
                <xsl:copy-of select="current()"/>
              </Hit>
            </xsl:if>
          </xsl:for-each>
        </xsl:for-each>
      </Hits>
    </xsl:variable>

    <xsl:variable name="FoundSkytter">
      <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/SkytterNavn"/>
    </xsl:variable>


    <xsl:copy>
      <xsl:choose>
        <xsl:when test="string($FoundSkytter) !='' and string($LagskytingRapport)='false'">
          <xsl:attribute name="ref">
            <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/Skytter/@ref"/>
          </xsl:attribute>
        </xsl:when>
        <xsl:when test="string($FoundSkytter) !='' and string($LagskytingRapport)='true'">
          <xsl:attribute name="ref">
            <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/Skytter/@ref"/>
          </xsl:attribute>
        </xsl:when>

      </xsl:choose>
      <xsl:apply-templates select="@*|node()"/>

    </xsl:copy>
  </xsl:template>
  <!-- this template is applied to a path node that doesn't have a fill attribute -->

  <xsl:template match="result[not(@ref)]">
    <!-- copy me and my attributes and my subnodes, applying templates as necessary, and add a fill attribute set to red -->


    <xsl:variable name="LagskytingRapport">
      <xsl:choose>
        <xsl:when test="/Merged/report/header/@name='Ungdom'">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:when test="/Merged/report/header/@name='Ungdom individuell'">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:when test="/Merged/report/header/@name='Veteran'">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:when test="/Merged/report/header/@name='Veteran individuell'">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:when test="/Merged/report/header/@name='Senior'">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:when test="/Merged/report/header/@name='Senior individuell'">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="boolean(0)"/>
        </xsl:otherwise>
      </xsl:choose>


    </xsl:variable>

     <xsl:variable name="MinneSkytingRapport">
      <xsl:choose>
        <xsl:when test="/Merged/report/header/@group_name='Minneskyting'">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="boolean(0)"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>


    <xsl:variable name="ClassValue">
      <xsl:value-of select="@class"/>
    </xsl:variable>
    <xsl:variable name="UseClass">
      <xsl:choose>
        <xsl:when test="string($ClassValue)!=''">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="boolean(0)"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>

    <xsl:variable name="CurrentSkytter">
      <xsl:value-of select="concat(utils:TuUpper(@name),utils:TuUpper(@club),utils:TuUpper(@class))"/>
    </xsl:variable>

    <xsl:variable name="FoundSkyttere">
      <Hits>
        <xsl:for-each select="/Merged/ArrayOfStartingListSkive" >
          <xsl:for-each select="StartingListSkive" >
            <xsl:variable name="CurrentRowSkytter">
              <xsl:choose>
                <xsl:when test="string($UseClass)='true'">
                  <xsl:value-of select="concat(utils:TuUpper(SkytterNavn),utils:TuUpper(SkytterLag),utils:TuUpper(Klasse))"/>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="concat(utils:TuUpper(SkytterNavn),utils:TuUpper(SkytterLag))"/>
                </xsl:otherwise>
              </xsl:choose>

            </xsl:variable>

            <xsl:if test="$CurrentSkytter = $CurrentRowSkytter">
              <Hit>
                <xsl:copy-of select="current()"/>
              </Hit>
            </xsl:if>
          </xsl:for-each>
        </xsl:for-each>
      </Hits>
    </xsl:variable>

    <xsl:variable name="FoundSkytter">
      <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/SkytterNavn"/>
    </xsl:variable>


    <xsl:copy>
      <xsl:choose>
        <xsl:when test="string($FoundSkytter) !='' and string($LagskytingRapport)='false'">
          <xsl:if test="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/BackUpBitMapFileName != ''">
            <xsl:attribute name="ref">
              <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/BackUpBitMapFileName"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:if test="string($MinneSkytingRapport)='true' and msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/BackUpBitMapFileNameMinne != ''">
            <xsl:attribute name="ref">
              <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/BackUpBitMapFileNameMinne"/>
            </xsl:attribute>
          </xsl:if>
        </xsl:when>
        <xsl:when test="string($FoundSkytter) !='' and string($LagskytingRapport)='true'">
          <xsl:if test="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/BackUpBitMapFileName != ''">
            <xsl:attribute name="ref">
              <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/BackUpBitMapFileName"/>
            </xsl:attribute>
          </xsl:if>
        </xsl:when>

      </xsl:choose>

      <xsl:apply-templates select="@*|node()"/>

    </xsl:copy>
  </xsl:template>

  <xsl:template match="result/series[@ref]">
    <!-- copy me and my attributes and my subnodes, applying templates as necessary, and add a fill attribute set to red -->


    <xsl:variable name="LagskytingRapport">
      <xsl:choose>
        <xsl:when test="/Merged/report/header/@name='Ungdom'">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:when test="/Merged/report/header/@name='Ungdom individuell'">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:when test="/Merged/report/header/@name='Veteran'">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:when test="/Merged/report/header/@name='Veteran individuell'">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:when test="/Merged/report/header/@name='Senior'">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:when test="/Merged/report/header/@name='Senior individuell'">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="boolean(0)"/>
        </xsl:otherwise>
      </xsl:choose>


    </xsl:variable>



    <xsl:variable name="ClassValue">
      <xsl:value-of select="ancestor::result[1]/@class"/>
    </xsl:variable>
    <xsl:variable name="UseClass">
      <xsl:choose>
        <xsl:when test="string($ClassValue)!=''">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="boolean(0)"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>

    <xsl:variable name="CurrentSkytter">
       <xsl:value-of select="concat(utils:TuUpper(ancestor::result[1]/@name),utils:TuUpper(ancestor::result[1]/@club),utils:TuUpper(ancestor::result[1]/@class))"/>
    </xsl:variable>

    <xsl:variable name="FoundSkyttere">
      <Hits>
        <xsl:for-each select="/Merged/ArrayOfStartingListSkive" >
          <xsl:for-each select="StartingListSkive" >
            <xsl:variable name="CurrentRowSkytter">
              <xsl:choose>
                <xsl:when test="string($UseClass)='true'">
                  <xsl:value-of select="concat(utils:TuUpper(SkytterNavn),utils:TuUpper(SkytterLag),utils:TuUpper(Klasse))"/>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="concat(utils:TuUpper(SkytterNavn),utils:TuUpper(SkytterLag))"/>
                </xsl:otherwise>
              </xsl:choose>

            </xsl:variable>

            <xsl:if test="$CurrentSkytter = $CurrentRowSkytter">
              <Hit>
                <xsl:copy-of select="current()"/>
              </Hit>
            </xsl:if>
          </xsl:for-each>
        </xsl:for-each>
      </Hits>
    </xsl:variable>

    <xsl:variable name="FoundSkytter">
      <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/SkytterNavn"/>
    </xsl:variable>


    <xsl:copy>
      <xsl:choose>
        <xsl:when test="string($FoundSkytter) !='' and string($LagskytingRapport)='false'">
          <xsl:choose>
            <xsl:when test="number(@id) = 1">
              <xsl:if test="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/AllSerier/Serie[SerieNr='1']/BackUpBitMapFileName != ''">
                <xsl:attribute name="ref">
                  <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/AllSerier/Serie[SerieNr='1']/BackUpBitMapFileName"/>
                </xsl:attribute>
              </xsl:if>
            </xsl:when>
            <xsl:when test="number(@id) = 2">
               <xsl:if test="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/AllSerier/Serie[SerieNr='2']/BackUpBitMapFileName != ''">
                <xsl:attribute name="ref">
                  <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/AllSerier/Serie[SerieNr='2']/BackUpBitMapFileName"/>
                </xsl:attribute>
              </xsl:if>
            </xsl:when>
            <xsl:when test="number(@id) = 3">
              <xsl:if test="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/AllSerier/Serie[SerieNr='3']/BackUpBitMapFileName != ''">
                <xsl:attribute name="ref">
                  <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/AllSerier/Serie[SerieNr='3']/BackUpBitMapFileName"/>
                </xsl:attribute>
              </xsl:if>
            </xsl:when>
            <xsl:when test="number(@id) = 4">
              <xsl:if test="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/AllSerier/Serie[SerieNr='4']/BackUpBitMapFileName != ''">
                <xsl:attribute name="ref">
                  <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/AllSerier/Serie[SerieNr='4']/BackUpBitMapFileName"/>
                </xsl:attribute>
              </xsl:if>
            </xsl:when>
            <xsl:when test="number(@id) = 5">
              <xsl:if test="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/AllSerier/Serie[SerieNr='5']/BackUpBitMapFileName != ''">
                <xsl:attribute name="ref">
                  <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/AllSerier/Serie[SerieNr='5']/BackUpBitMapFileName"/>
                </xsl:attribute>
              </xsl:if>
            </xsl:when>
           <xsl:when test="number(@id) = 7">
              <xsl:if test="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/BackUpBitMapFileNameFinale != ''">
                <xsl:attribute name="ref">
                  <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/BackUpBitMapFileNameFinale"/>
                </xsl:attribute>
              </xsl:if>
            </xsl:when>
            <xsl:when test="number(@id) = 8">
              <xsl:if test="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/BackUpBitMapFileNameFinale != ''">
                <xsl:attribute name="ref">
                  <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/BackUpBitMapFileNameFinale"/>
                </xsl:attribute>
              </xsl:if>
            </xsl:when>
          </xsl:choose>
        </xsl:when>
        <xsl:when test="string($FoundSkytter) !='' and string($LagskytingRapport)='true'">
          <xsl:choose>
            <xsl:when test="number(@id) = 1">
              <xsl:if test="msxsl:node-set($FoundSkyttere)/Hits/Hit/Skytter/@ref1 != ''">
                <xsl:attribute name="ref">
                  <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/Skytter/@ref1"/>
                </xsl:attribute>
              </xsl:if>
            </xsl:when>
            <xsl:when test="number(@id) = 2">
              <xsl:if test="msxsl:node-set($FoundSkyttere)/Hits/Hit/Skytter/@ref2 != ''">
                <xsl:attribute name="ref">
                  <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/Skytter/@ref2"/>
                </xsl:attribute>
              </xsl:if>
            </xsl:when>
            <xsl:when test="number(@id) = 3">
              <xsl:if test="msxsl:node-set($FoundSkyttere)/Hits/Hit/Skytter/@ref3 != ''">
                <xsl:attribute name="ref">
                  <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/Skytter/@ref3"/>
                </xsl:attribute>
              </xsl:if>
            </xsl:when>
            <xsl:when test="number(@id) = 4">
              <xsl:if test="msxsl:node-set($FoundSkyttere)/Hits/Hit/Skytter/@ref4 != ''">
                <xsl:attribute name="ref">
                  <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/Skytter/@ref4"/>
                </xsl:attribute>
              </xsl:if>
            </xsl:when>
            <xsl:when test="number(@id) = 5">
              <xsl:if test="msxsl:node-set($FoundSkyttere)/Hits/Hit/Skytter/@ref5 != ''">
                <xsl:attribute name="ref">
                  <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/Skytter/@ref5"/>
                </xsl:attribute>
              </xsl:if>
            </xsl:when>
          </xsl:choose>
        </xsl:when>

      </xsl:choose>

      <xsl:apply-templates select="@*|node()"/>

    </xsl:copy>
  </xsl:template>
    
    
  <xsl:template match="result/series[not(@ref)]">
    <!-- copy me and my attributes and my subnodes, applying templates as necessary, and add a fill attribute set to red -->


    <xsl:variable name="LagskytingRapport">
      <xsl:choose>
        <xsl:when test="/Merged/report/header/@name='Ungdom'">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:when test="/Merged/report/header/@name='Ungdom individuell'">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:when test="/Merged/report/header/@name='Veteran'">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:when test="/Merged/report/header/@name='Veteran individuell'">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:when test="/Merged/report/header/@name='Senior'">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:when test="/Merged/report/header/@name='Senior individuell'">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="boolean(0)"/>
        </xsl:otherwise>
      </xsl:choose>


    </xsl:variable>

  <xsl:variable name="MinneSkytingRapport">
      <xsl:choose>
        <xsl:when test="/Merged/report/header/@group_name='Minneskyting'">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="boolean(0)"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>

    <xsl:variable name="ClassValue">
      <xsl:value-of select="ancestor::result[1]/@class"/>
    </xsl:variable>
    <xsl:variable name="UseClass">
      <xsl:choose>
        <xsl:when test="string($ClassValue)!=''">
          <xsl:value-of select="boolean(1)"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="boolean(0)"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>

    <xsl:variable name="CurrentSkytter">
      <xsl:value-of select="concat(utils:TuUpper(ancestor::result[1]/@name),utils:TuUpper(ancestor::result[1]/@club),utils:TuUpper(ancestor::result[1]/@class))"/>
    </xsl:variable>

    <xsl:variable name="FoundSkyttere">
      <Hits>
        <xsl:for-each select="/Merged/ArrayOfStartingListSkive" >
          <xsl:for-each select="StartingListSkive" >
            <xsl:variable name="CurrentRowSkytter">
              <xsl:choose>
                <xsl:when test="string($UseClass)='true'">
                  <xsl:value-of select="concat(utils:TuUpper(SkytterNavn),utils:TuUpper(SkytterLag),utils:TuUpper(Klasse))"/>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="concat(utils:TuUpper(SkytterNavn),utils:TuUpper(SkytterLag))"/>
                </xsl:otherwise>
              </xsl:choose>

            </xsl:variable>

            <xsl:if test="$CurrentSkytter = $CurrentRowSkytter">
              <Hit>
                <xsl:copy-of select="current()"/>
              </Hit>
            </xsl:if>
          </xsl:for-each>
        </xsl:for-each>
      </Hits>
    </xsl:variable>

    <xsl:variable name="FoundSkytter">
      <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/SkytterNavn"/>
    </xsl:variable>


    <xsl:copy>
      <xsl:choose>
        <xsl:when test="string($FoundSkytter) !='' and string($LagskytingRapport)='false' and string($MinneSkytingRapport)='false'">
          <xsl:choose>
            <xsl:when test="number(@id) = 1">
              <xsl:if test="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/AllSerier/Serie[SerieNr='1']/BackUpBitMapFileName != ''">
                <xsl:attribute name="ref">
                  <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/AllSerier/Serie[SerieNr='1']/BackUpBitMapFileName"/>
                </xsl:attribute>
              </xsl:if>
            </xsl:when>
            <xsl:when test="number(@id) = 2">
               <xsl:if test="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/AllSerier/Serie[SerieNr='2']/BackUpBitMapFileName != ''">
                <xsl:attribute name="ref">
                  <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/AllSerier/Serie[SerieNr='2']/BackUpBitMapFileName"/>
                </xsl:attribute>
              </xsl:if>
            </xsl:when>
            <xsl:when test="number(@id) = 3">
              <xsl:if test="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/AllSerier/Serie[SerieNr='3']/BackUpBitMapFileName != ''">
                <xsl:attribute name="ref">
                  <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/AllSerier/Serie[SerieNr='3']/BackUpBitMapFileName"/>
                </xsl:attribute>
              </xsl:if>
            </xsl:when>
            <xsl:when test="number(@id) = 4">
              <xsl:if test="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/AllSerier/Serie[SerieNr='4']/BackUpBitMapFileName != ''">
                <xsl:attribute name="ref">
                  <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/AllSerier/Serie[SerieNr='4']/BackUpBitMapFileName"/>
                </xsl:attribute>
              </xsl:if>
            </xsl:when>
            <xsl:when test="number(@id) = 5">
              <xsl:if test="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/AllSerier/Serie[SerieNr='5']/BackUpBitMapFileName != ''">
                <xsl:attribute name="ref">
                  <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/AllSerier/Serie[SerieNr='5']/BackUpBitMapFileName"/>
                </xsl:attribute>
              </xsl:if>
            </xsl:when>
            <xsl:when test="number(@id) = 7">
              <xsl:if test="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/BackUpBitMapFileNameFinale != ''">
                <xsl:attribute name="ref">
                  <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/BackUpBitMapFileNameFinale"/>
                </xsl:attribute>
              </xsl:if>
            </xsl:when>
            <xsl:when test="number(@id) = 8">
              <xsl:if test="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/BackUpBitMapFileNameFinale != ''">
                <xsl:attribute name="ref">
                  <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/StartingListSkive/BackUpBitMapFileNameFinale"/>
                </xsl:attribute>
              </xsl:if>
            </xsl:when>
          </xsl:choose>
        </xsl:when>
        <xsl:when test="string($FoundSkytter) !='' and string($LagskytingRapport)='true'">
          <xsl:choose>
            <xsl:when test="number(@id) = 1">
              <xsl:if test="msxsl:node-set($FoundSkyttere)/Hits/Hit/Skytter/@ref1 != ''">
                <xsl:attribute name="ref">
                  <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/Skytter/@ref1"/>
                </xsl:attribute>
              </xsl:if>
            </xsl:when>
            <xsl:when test="number(@id) = 2">
              <xsl:if test="msxsl:node-set($FoundSkyttere)/Hits/Hit/Skytter/@ref2 != ''">
                <xsl:attribute name="ref">
                  <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/Skytter/@ref2"/>
                </xsl:attribute>
              </xsl:if>
            </xsl:when>
            <xsl:when test="number(@id) = 3">
              <xsl:if test="msxsl:node-set($FoundSkyttere)/Hits/Hit/Skytter/@ref3 != ''">
                <xsl:attribute name="ref">
                  <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/Skytter/@ref3"/>
                </xsl:attribute>
              </xsl:if>
            </xsl:when>
            <xsl:when test="number(@id) = 4">
              <xsl:if test="msxsl:node-set($FoundSkyttere)/Hits/Hit/Skytter/@ref4 != ''">
                <xsl:attribute name="ref">
                  <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/Skytter/@ref4"/>
                </xsl:attribute>
              </xsl:if>
            </xsl:when>
            <xsl:when test="number(@id) = 5">
              <xsl:if test="msxsl:node-set($FoundSkyttere)/Hits/Hit/Skytter/@ref5 != ''">
                <xsl:attribute name="ref">
                  <xsl:value-of select="msxsl:node-set($FoundSkyttere)/Hits/Hit/Skytter/@ref5"/>
                </xsl:attribute>
              </xsl:if>
            </xsl:when>
          </xsl:choose>
        </xsl:when>

      </xsl:choose>

      <xsl:apply-templates select="@*|node()"/>

    </xsl:copy>
  </xsl:template>


</xsl:stylesheet>
