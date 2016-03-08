<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
      xmlns:msxsl="urn:schemas-microsoft-com:xslt"
     exclude-result-prefixes="msxsl">
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="/">
    <Skyttere>
      <xsl:apply-templates select="/Merged/report" />
    </Skyttere>
  </xsl:template>
  <xsl:template match="report">
    <xsl:apply-templates select="/Merged/report/data/result" />
  </xsl:template>
  
  <xsl:template match="result">
    <xsl:variable name ="Lagnr">
      <xsl:value-of select="substring-after(/Merged/report/header/@name,'Lag ')"/>
    </xsl:variable>

    <xsl:variable name ="skivenr">
      <xsl:value-of select="@num"/>
    </xsl:variable>

    <xsl:variable name ="filenameToFind">
      <xsl:value-of select="concat('TR-',$Lagnr,'-',$skivenr,'.PNG')"/>
    </xsl:variable>

    <xsl:variable name="FoundBitmaps">
      <Hits>
        <xsl:for-each select="/Merged/BitmapDirInfo/Bitmaps/FileName" >
          <xsl:if test="current() = $filenameToFind">
            <Hit>
              <xsl:value-of select="current()"/>
            </Hit>
          </xsl:if>
        </xsl:for-each>
      </Hits>
    </xsl:variable>

    <xsl:variable name="FoundBitmap">
      <xsl:value-of select="msxsl:node-set($FoundBitmaps)/Hits/Hit"/>
    </xsl:variable>

    
    <xsl:variable name="SkytterName">
      <xsl:value-of select="@name"/>
     </xsl:variable> 
    <xsl:variable name="SkytterClub">
      <xsl:value-of select="@club"/>
     </xsl:variable> 
       <xsl:variable name="SkytterClass">
      <xsl:value-of select="@class"/>
     </xsl:variable> 
    
    <!-- produce a fill attribute with content "red" -->
    <xsl:choose>
    <xsl:when test="string($FoundBitmap) !=''">
      <xsl:element name="Skytter">
        <xsl:attribute name="name">
          <xsl:value-of select="$SkytterName"/>
        </xsl:attribute>
        <xsl:attribute name="club">
          <xsl:value-of select="$SkytterClub"/>
        </xsl:attribute>
        <xsl:attribute name="class">
          <xsl:value-of select="$SkytterClass"/>
        </xsl:attribute>
        <xsl:attribute name="ref">
          <xsl:value-of select="concat(/Merged/BitmapDirInfo/BitmapSubDir,'/',$FoundBitmap)"/>
        </xsl:attribute>
      </xsl:element>
    </xsl:when>
    <xsl:otherwise>
       <xsl:element name="Skytter">
                  <xsl:attribute name="name">
                    <xsl:value-of select="$SkytterName"/>
                  </xsl:attribute>
                  <xsl:attribute name="club">
                    <xsl:value-of select="$SkytterClub"/>
                  </xsl:attribute>
                  <xsl:attribute name="class">
                    <xsl:value-of select="$SkytterClass"/>
                  </xsl:attribute>
        <xsl:for-each select="series">
             <xsl:variable name ="serienr">
                  <xsl:value-of select="@id"/>
              </xsl:variable>
             <xsl:variable name ="filenameSerieToFind">
              <xsl:value-of select="concat('TR-',$Lagnr,'-',$skivenr,'-',$serienr,'.PNG')"/>
            </xsl:variable>
             <xsl:variable name="FoundBitmapsSerie">
              <Hits>
                <xsl:for-each select="/Merged/BitmapDirInfo/Bitmaps/FileName" >
                  <xsl:if test="current() = $filenameSerieToFind">
                    <Hit>
                      <xsl:value-of select="current()"/>
                    </Hit>
                  </xsl:if>
                </xsl:for-each>
              </Hits>
            </xsl:variable>

            <xsl:variable name="FoundBitmapSerie">
              <xsl:value-of select="msxsl:node-set($FoundBitmapsSerie)/Hits/Hit"/>
            </xsl:variable>
          <xsl:variable name ="attributeName">
                  <xsl:value-of select="concat('ref',$serienr)"/>
              </xsl:variable>
               <xsl:if test="string($FoundBitmapSerie) !=''">
                 <xsl:choose>
                   <xsl:when test="number($serienr)=1">
                      <xsl:attribute name="ref1">
                         <xsl:value-of select="concat(/Merged/BitmapDirInfo/BitmapSubDir,'/',$FoundBitmapSerie)"/>
                     </xsl:attribute>
                   </xsl:when>
                   <xsl:when test="number($serienr)=2">
                      <xsl:attribute name="ref2">
                         <xsl:value-of select="concat(/Merged/BitmapDirInfo/BitmapSubDir,'/',$FoundBitmapSerie)"/>
                     </xsl:attribute>
                   </xsl:when>
                   <xsl:when test="number($serienr)=3">
                      <xsl:attribute name="ref3">
                         <xsl:value-of select="concat(/Merged/BitmapDirInfo/BitmapSubDir,'/',$FoundBitmapSerie)"/>
                     </xsl:attribute>
                   </xsl:when>
                   <xsl:when test="number($serienr)=4">
                      <xsl:attribute name="ref4">
                         <xsl:value-of select="concat(/Merged/BitmapDirInfo/BitmapSubDir,'/',$FoundBitmapSerie)"/>
                     </xsl:attribute>
                   </xsl:when>
                   <xsl:when test="number($serienr)=5">
                      <xsl:attribute name="ref5">
                         <xsl:value-of select="concat(/Merged/BitmapDirInfo/BitmapSubDir,'/',$FoundBitmapSerie)"/>
                     </xsl:attribute>
                   </xsl:when>
                 </xsl:choose>
                </xsl:if>
        </xsl:for-each>
          </xsl:element>
      </xsl:otherwise>
  </xsl:choose>
  </xsl:template>

</xsl:stylesheet>
