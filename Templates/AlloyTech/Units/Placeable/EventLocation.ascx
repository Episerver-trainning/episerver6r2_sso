<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EventLocation.ascx.cs"
    Inherits="EPiServer.Templates.AlloyTech.Units.Placeable.EventLocation" %>
    
<asp:PlaceHolder runat="server" ID="LocationPlaceHolder">
    <div>
        <h3>
            <%= Translate("/templates/events/location") %>
        </h3>
        <EPiServer:Property PropertyName="Location" runat="server" CustomTagName="p" />
    </div>
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="SpeakerPlaceHolder">
    <div>
        <h3>
            <%= Translate("/templates/events/speaker") %>
        </h3>
        <EPiServer:Property PropertyName="Speaker" runat="server" CustomTagName="p" />
    </div>
</asp:PlaceHolder>
