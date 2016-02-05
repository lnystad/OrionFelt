<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
    <xsl:output method="xml" indent="yes" encoding="utf-8"/>

    <xsl:template match="/">
  
  <paamelding stevnenavn="TestStevne FinFeltNNM" stevnenummer="15047" arrangor="10584" stevnestart="03.03.2015" stevneslutt="05.03.2015">
    <klasse navn="V55" arravgift="0">
      <premiering id="0" navn="10sk" innskudd="0" />
    </klasse>
    <klasse navn="V65" arravgift="0">
      <premiering id="0" navn="10sk" innskudd="0" />
    </klasse>
    <klasse navn="V73" arravgift="0">
      <premiering id="0" navn="10sk" innskudd="0" />
    </klasse>
    <klasse navn="1" arravgift="0">
      <premiering id="0" navn="10sk" innskudd="0" />
    </klasse>
    <klasse navn="2" arravgift="0">
      <premiering id="0" navn="10sk" innskudd="0" />
    </klasse>
    <klasse navn="4" arravgift="0">
      <premiering id="0" navn="10sk" innskudd="0" />
    </klasse>
    <klasse navn="5" arravgift="0">
      <premiering id="0" navn="10sk" innskudd="0" />
    </klasse>
    <ovelse id="FE" hold="Fin felt" stang="1" felthurtig="1" minne="1">
      <xsl:for-each select="/ArrayOfLag/Lag">

        <xsl:variable name="Dato">
          <xsl:value-of select="concat(substring(LagTid,9,2),'.',substring(LagTid,6,2),'.', substring(LagTid,1,4))"/>
        </xsl:variable>
        <xsl:variable name="Time">
          <xsl:value-of select="substring(LagTid,12,5)"/>
        </xsl:variable>
        <lag navn="" lagnr="{LagNummer}" dato="{$Dato}" kl-opprop="" kl-skytetid="{$Time}" />
      </xsl:for-each>

      <xsl:for-each select="/ArrayOfLag/Lag">
        <xsl:variable name="LagNr">
          <xsl:value-of select="LagNummer"/>
        </xsl:variable>
        <xsl:for-each select="SkiverILaget/Skiver">
          
          <xsl:variable name="SkiveNr">
            <xsl:value-of select="SkiveNummer"/>
          </xsl:variable>
          <xsl:if test="Skytter/Id!=''">
          <paamelding-skytter medlemsid="{Skytter/SkytterNr}" fornavn="{Skytter/Fornavn}" etternavn="{Skytter/EtterNavn}" sklag-nr="{Skytter/SkytterlagNr}" lag="{$LagNr}" skive="{$SkiveNr}" klasse="{Skytter/Klasse}" klasse-felt="{Skytter/Klasse}" klasse-skogslop="" kat-mf="0" kat-mb="1" kat-k="0" kat-l="0" kat-a="0" kat-n="0" kat-ft="" kat-22="0" kat-luft="0" gruppe="" />
          </xsl:if>
        </xsl:for-each>
      </xsl:for-each>
      <!--<lag navn="" lagnr="1" dato="03.03.2015" kl-opprop="" kl-skytetid="18:00" />
      <lag navn="" lagnr="2" dato="03.03.2015" kl-opprop="" kl-skytetid="18:45" />
      <lag navn="" lagnr="3" dato="03.03.2015" kl-opprop="" kl-skytetid="19:30" />
      <lag navn="" lagnr="4" dato="03.03.2015" kl-opprop="" kl-skytetid="20:15" />
      <lag navn="" lagnr="5" dato="05.03.2015" kl-opprop="" kl-skytetid="18:00" />
      <lag navn="" lagnr="6" dato="05.03.2015" kl-opprop="" kl-skytetid="18:45" />-->
      <!--<paamelding-skytter medlemsid="1651777" fornavn="Jan Erik" etternavn="Aasheim" sklag-nr="10584" epost="je-aas@online.no" faar="1972" lag="3" skive="9" klasse="5" klasse-felt="" klasse-skogslop="" kat-mf="0" kat-mb="1" kat-k="0" kat-l="0" kat-a="0" kat-n="0" kat-ft="" kat-22="0" kat-luft="0" gruppe="" />
      <paamelding-skytter medlemsid="1651843" fornavn="Svein" etternavn="Andreassen" sklag-nr="10584" epost="s-johaan@online.no" faar="1943" lag="4" skive="1" klasse="V55" klasse-felt="" klasse-skogslop="" kat-mf="0" kat-mb="0" kat-k="0" kat-l="0" kat-a="0" kat-n="0" kat-ft="" kat-22="0" kat-luft="0" gruppe="" />
      <paamelding-skytter medlemsid="1652031" fornavn="Magnus" etternavn="Lund" sklag-nr="10584" epost="" faar="1965" lag="3" skive="5" klasse="V55" klasse-felt="" klasse-skogslop="" kat-mf="0" kat-mb="0" kat-k="0" kat-l="0" kat-a="0" kat-n="0" kat-ft="" kat-22="0" kat-luft="0" gruppe="" />
      <paamelding-skytter medlemsid="1652452" fornavn="Bård" etternavn="Pedersen" sklag-nr="10584" epost="bard@nordkontakt.no" faar="1962" lag="3" skive="10" klasse="4" klasse-felt="" klasse-skogslop="" kat-mf="0" kat-mb="0" kat-k="0" kat-l="0" kat-a="0" kat-n="0" kat-ft="" kat-22="0" kat-luft="0" gruppe="" />
      <paamelding-skytter medlemsid="1652536" fornavn="Magne" etternavn="Drægni" sklag-nr="10584" epost="ma-draeg@online.no" faar="1943" lag="3" skive="1" klasse="V65" klasse-felt="" klasse-skogslop="" kat-mf="0" kat-mb="0" kat-k="0" kat-l="0" kat-a="0" kat-n="0" kat-ft="" kat-22="0" kat-luft="0" gruppe="" />
      <paamelding-skytter medlemsid="1652676" fornavn="Hans Kristian" etternavn="Øvermark" sklag-nr="10584" epost="" faar="1939" lag="3" skive="4" klasse="V73" klasse-felt="" klasse-skogslop="" kat-mf="0" kat-mb="0" kat-k="0" kat-l="0" kat-a="0" kat-n="0" kat-ft="" kat-22="0" kat-luft="0" gruppe="" />
      <paamelding-skytter medlemsid="1653773" fornavn="Magnus Skyrud" etternavn="Jensen" sklag-nr="10584" epost="magnussj@live.no" faar="1993" lag="3" skive="14" klasse="4" klasse-felt="" klasse-skogslop="" kat-mf="0" kat-mb="0" kat-k="0" kat-l="0" kat-a="0" kat-n="0" kat-ft="" kat-22="0" kat-luft="0" gruppe="" />
      <paamelding-skytter medlemsid="1654003" fornavn="Stig Rune" etternavn="Valen" sklag-nr="10584" epost="sr-valen@online.no" faar="1963" lag="3" skive="13" klasse="2" klasse-felt="" klasse-skogslop="" kat-mf="0" kat-mb="0" kat-k="0" kat-l="0" kat-a="0" kat-n="0" kat-ft="" kat-22="0" kat-luft="0" gruppe="" />
      <paamelding-skytter medlemsid="1654425" fornavn="Stig" etternavn="Sørensen" sklag-nr="10584" epost="sorensen.stig@gmail.com" faar="1963" lag="4" skive="11" klasse="2" klasse-felt="" klasse-skogslop="" kat-mf="0" kat-mb="0" kat-k="0" kat-l="0" kat-a="0" kat-n="0" kat-ft="" kat-22="0" kat-luft="0" gruppe="" />
      <paamelding-skytter medlemsid="1654599" fornavn="Emma" etternavn="Sørensen" sklag-nr="10584" epost="sorensen_96@hotmail.com" faar="1996" lag="4" skive="9" klasse="2" klasse-felt="" klasse-skogslop="" kat-mf="0" kat-mb="0" kat-k="1" kat-l="0" kat-a="0" kat-n="0" kat-ft="" kat-22="0" kat-luft="0" gruppe="" />
      <paamelding-skytter medlemsid="1654979" fornavn="Berit Pauline" etternavn="Pedersen" sklag-nr="10584" epost="Berit.pedersen@nfk.no" faar="1963" lag="3" skive="11" klasse="1" klasse-felt="" klasse-skogslop="" kat-mf="0" kat-mb="0" kat-k="1" kat-l="0" kat-a="0" kat-n="0" kat-ft="" kat-22="0" kat-luft="0" gruppe="" />
      <paamelding-skytter medlemsid="1728443" fornavn="Øystein" etternavn="Nyhagen" sklag-nr="10584" epost="Nyhoys@online.no" faar="1967" lag="4" skive="5" klasse="4" klasse-felt="" klasse-skogslop="" kat-mf="0" kat-mb="0" kat-k="0" kat-l="0" kat-a="0" kat-n="0" kat-ft="" kat-22="0" kat-luft="0" gruppe="" />
      <paamelding-skytter medlemsid="2094985" fornavn="Julie Simonette" etternavn="Lian" sklag-nr="10584" epost="" faar="1994" lag="4" skive="7" klasse="1" klasse-felt="" klasse-skogslop="" kat-mf="0" kat-mb="0" kat-k="1" kat-l="0" kat-a="0" kat-n="0" kat-ft="" kat-22="0" kat-luft="0" gruppe="" />
      <paamelding-skytter medlemsid="2170223" fornavn="Terje" etternavn="Holten" sklag-nr="10584" epost="tooterje@online.no" faar="1975" lag="3" skive="8" klasse="2" klasse-felt="" klasse-skogslop="" kat-mf="0" kat-mb="0" kat-k="0" kat-l="0" kat-a="0" kat-n="0" kat-ft="" kat-22="0" kat-luft="0" gruppe="" />
      <paamelding-skytter medlemsid="2316412" fornavn="Lene Mari" etternavn="Sæthre" sklag-nr="10584" epost="lene@soerblomst.no" faar="1974" lag="4" skive="14" klasse="2" klasse-felt="" klasse-skogslop="" kat-mf="0" kat-mb="0" kat-k="1" kat-l="0" kat-a="0" kat-n="0" kat-ft="" kat-22="0" kat-luft="0" gruppe="" />-->
    </ovelse>
  </paamelding>
   </xsl:template>
</xsl:stylesheet>
