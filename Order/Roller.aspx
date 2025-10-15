<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Roller.aspx.vb" Inherits="Order_Roller" MaintainScrollPositionOnPostback="true" MasterPageFile="~/Site.master" Debug="true" Title="Roller Order" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">
                        <span runat="server" id="pageAction"></span>
                    </div>
                    <h2 class="page-title" runat="server" id="pageTitle"></h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row mb-3">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-lg-12">
                                    <asp:Table runat="server" Font-Size="Larger" CellPadding="5">
                                        <asp:TableRow>
                                            <asp:TableCell Width="130px">Order Number</asp:TableCell>
                                            <asp:TableCell Width="15px">:</asp:TableCell>
                                            <asp:TableCell>
                                                <asp:Label runat="server" ID="lblOrderNo"></asp:Label>
                                            </asp:TableCell>
                                        </asp:TableRow>
                                        <asp:TableRow>
                                            <asp:TableCell>Reference</asp:TableCell>
                                            <asp:TableCell>:</asp:TableCell>
                                            <asp:TableCell>
                                                <asp:Label runat="server" ID="lblOrderCust"></asp:Label>
                                            </asp:TableCell>
                                        </asp:TableRow>
                                    </asp:Table>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-7 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title" runat="server" id="cardTitle"></h3>
                            <div class="card-actions">
                                <asp:LinkButton runat="server" ID="btnReset" CssClass="btn btn-danger w-100 "  OnClick="btnReset_Click">
                                    <i class="fa-solid fa-rotate me-2"></i>Reset
                                </asp:LinkButton>
                            </div>
                        </div>

                        <div class="card-body">
                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">ROLLER TYPE</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlBlindType" CssClass="form-select" Font-Bold="true" AutoPostBack="true" OnSelectedIndexChanged="ddlBlindType_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-3 row" runat="server" id="divBracketType">
                                <asp:Label runat="server" ID="lblBracketType" CssClass="col-lg-3 col-form-label">BRACKET TYPE</asp:Label>
                                <div class="col-lg-5 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlBracketType" CssClass="form-select" Font-Bold="true" AutoPostBack="true" OnSelectedIndexChanged="ddlBracketType_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-3 row" runat="server" id="divTubeType">
                                <asp:Label class="col-lg-3 col-form-label" runat="server" ID="lblTubeType">MECHANISM TYPE</asp:Label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlTubeType" CssClass="form-select" Font-Bold="true" AutoPostBack="true" OnSelectedIndexChanged="ddlTubeType_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-3 row" runat="server" id="divControlType">
                                <label class="col-lg-3 col-form-label">CONTROL TYPE</label>
                                <div class="col-lg-3 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlControlType" CssClass="form-select" Font-Bold="true" AutoPostBack="true" OnSelectedIndexChanged="ddlControlType_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-3 row" runat="server" id="divColourType">
                                <asp:Label class="col-lg-3 col-form-label" runat="server" ID="lblColourType">CONTROL COLOUR</asp:Label>
                                <div class="col-lg-3 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlColourType" CssClass="form-select" Font-Bold="true" AutoPostBack="true" OnSelectedIndexChanged="ddlColourType_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                            </div>

                            <div runat="server" id="divDetail">
                                <hr />
                                <div class="mb-3 row" runat="server" id="divAttention">
                                    <div class="col-lg-12">
                                        <div class="alert alert-danger" role="alert">
                                            <div class="d-flex">
                                                <div>
                                                    <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round">
                                                        <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
                                                        <path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" />
                                                        <path d="M12 9h.01" /><path d="M11 12h1v4h1" />
                                                    </svg>
                                                </div>
                                                <div>
                                                    <h4 class="alert-title">ATTENTION !</h4>
                                                    <div class="text-secondary">
                                                        <asp:Label runat="server" ID="lblNextDesc"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="mb-3 row">
                                    <label class="col-lg-3 col-form-label">QUANTITY</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:TextBox runat="server" TextMode="Number" ID="txtQty" CssClass="form-control qty" placeholder="Qty ..." autocomplete="off" data-clientid='<%= txtQty.ClientID %>'></asp:TextBox>
                                    </div>
                                    <div class="col-lg-2" runat="server" id="divQtyInfo">
                                        <a class="btn btn-primary " data-bs-toggle="offcanvas" href="#canvasInfo" role="button" aria-controls="canvasInfo" onclick="return showInfo('Quantity');"><i class="bi bi-info-circle me-2"></i>Info</a>
                                    </div>
                                </div>

                                <div class="mb-3 row">
                                    <label class="col-lg-3 col-form-label">ROOM TO INSTALL</label>
                                    <div class="col-lg-6 col-md-12 col-sm-12">
                                        <asp:TextBox runat="server" ID="txtLocation" CssClass="form-control" placeholder="Location ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="mb-6 row" runat="server" id="divMounting">
                                    <label class="col-lg-3 col-form-label">MOUNTING</label>
                                    <div class="col-lg-3 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlMounting" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Face Fit" Text="FACE FIT"></asp:ListItem>
                                            <asp:ListItem Value="Reveal Fit" Text="REVEAL FIT"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-3 mt-5 row" runat="server" id="divMotorStyle">
                                    <label class="col-lg-3 col-form-label">MOTOR STYLE</label>
                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlMotorStyle" CssClass="form-select" OnSelectedIndexChanged="ddlMotorStyle_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                    </div>
                                    <div class="col-lg-2" runat="server" id="divMotorStyleInfo">
                                        <a class="btn btn-primary" data-bs-toggle="offcanvas" href="#canvasInfo" role="button" aria-controls="canvasInfo" onclick="return showInfo('Motor Style');">Info</a>
                                    </div>
                                </div>

                                <div class="mb-6 row" runat="server" id="divMotorRemote">
                                    <label class="col-lg-3 col-form-label">MOTOR REMOTE</label>
                                    <div class="col-lg-5 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlMotorRemote" CssClass="form-select"></asp:DropDownList>
                                    </div>
                                    <div class="col-lg-2" runat="server" id="divMotorRemoteInfo">
                                        <a class="btn btn-primary" data-bs-toggle="offcanvas" href="#canvasInfo" role="button" aria-controls="canvasInfo" onclick="return showInfo('Motor Remote');">Info</a>
                                    </div>
                                </div>

                                <div class="mb-3 row" runat="server" id="divMotorBattery">
                                    <label class="col-lg-3 col-form-label">EXTERNAL BATTERY</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlExternalBattery" CssClass="form-select"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-3 row" runat="server" id="divMotorCharger">
                                    <label class="col-lg-3 col-form-label">CHARGER</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlMotorCharger" CssClass="form-select"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-3 row" runat="server" id="divCableExitPoint">
                                    <label class="col-lg-3 col-form-label">CABLE EXIT POINT</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlCableExitPoint" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Side" Text="SIDE"></asp:ListItem>
                                            <asp:ListItem Value="Top" Text="TOP"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-6 mt-6 row" runat="server" id="divConnector">
                                    <label class="col-lg-3 col-form-label">FLUSH CONNECT</label>
                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlConnector" CssClass="form-select" Width="100px">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Yes" Text="YES"></asp:ListItem>
                                        </asp:DropDownList>
                                        <small class="form-hint">* FLUSH CONNECTOR HOUSING</small>
                                    </div>
                                </div>

                                <div class="mb-3 mt-6 row" runat="server" id="divFabricType">
                                    <label class="col-lg-3 col-form-label">FABRIC TYPE</label>
                                    <div class="col-lg-5 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlFabricType" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlFabricType_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-5 row" runat="server" id="divFabricColour">
                                    <label class="col-lg-3 col-form-label">FABRIC COLOUR</label>
                                    <div class="col-lg-4 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlFabricColour" CssClass="form-select"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-3 row" runat="server" id="divRollDirection">
                                    <label class="col-lg-3 col-form-label">ROLL DIRECTION</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlRoll" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Reverse Roll" Text="REVERSE ROLL"></asp:ListItem>
                                            <asp:ListItem Value="Standard" Text="STANDARD"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-3 mt-6 row" runat="server" id="divControlPosition">
                                    <asp:Label runat="server" ID="lblControlPosition" CssClass="col-lg-3 col-form-label" Text="CONTROL POSITION"></asp:Label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlControlPosition" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Left" Text="LEFT"></asp:ListItem>
                                            <asp:ListItem Value="Right" Text="RIGHT"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-3 row" runat="server" id="divChainColour">
                                    <label class="col-lg-3 col-form-label">CHAIN COLOUR</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlChainColour" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Beige" Text="BEIGE"></asp:ListItem>
                                            <asp:ListItem Value="Birch White" Text="BIRCH WHITE"></asp:ListItem>
                                            <asp:ListItem Value="Black" Text="BLACK"></asp:ListItem>
                                            <asp:ListItem Value="Grey" Text="GREY"></asp:ListItem>
                                            <asp:ListItem Value="Stainless Steel" Text="STAINLESS STEEL"></asp:ListItem>
                                            <asp:ListItem Value="White" Text="WHITE"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-3 row" runat="server" id="divChainLength">
                                     <label class="col-lg-3 col-form-label">CHAIN LENGTH</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:TextBox runat="server" TextMode="Number" ID="txtChainLength" CssClass="form-control" placeholder="Length ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="mb-3 mt-6 row" runat="server" id="divTrim">
                                    <label class="col-lg-3 col-form-label">TRIM</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlTrim" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlTrim_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                    <div class="col-lg-4 col-md-12" runat="server" id="divTrimInfo">
                                        <a class="btn btn-primary " data-bs-toggle="offcanvas" href="#canvasInfo" role="button" aria-controls="canvasInfo" onclick="return showInfo('Trim');"><i class="bi bi-info-circle me-2"></i>Info</a>
                                    </div>
                                </div>

                                <div class="mb-3 mt-6 row" runat="server" id="divTrimSkin">
                                    <label class="col-lg-3 col-form-label">TRIM</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlTrimSkin" CssClass="form-select " AutoPostBack="true" OnSelectedIndexChanged="ddlTrimSkin_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-3 row" runat="server" id="divBottomRail">
                                    <label class="col-lg-3 col-form-label">BOTTOM RAIL</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlRailType" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlRailType_SelectedIndexChanged"></asp:DropDownList>
                                        <small class="form-hint">* Type</small>
                                    </div>
                                    
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlRailColour" CssClass="form-select"></asp:DropDownList>
                                        <small class="form-hint">* Colour</small>
                                    </div>
                                </div>
                                
                                <div class="mb-3 mt-6 row">
                                    <label class="col-lg-3 col-form-label">WIDTH x DROP</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:TextBox runat="server" TextMode="Number" min="1" ID="txtWidth" CssClass="form-control" placeholder="Width ..." autocomplete="off"></asp:TextBox>
                                        <small class="form-hint">* Width</small>
                                    </div>
                                    
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:TextBox runat="server" TextMode="Number" min="1" ID="txtDrop" CssClass="form-control" placeholder="Drop ..." autocomplete="off"></asp:TextBox>
                                        <small class="form-hint">* Drop</small>
                                    </div>
                                </div>

                                <div class="mb-3 mt-6 row" runat="server" id="divTubeSize">
                                    <label class="col-lg-3 col-form-label">TUBE SIZE</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlTubeSize" CssClass="form-select"></asp:DropDownList>
                                    </div>
                                    <div class="col-lg-2 col-md-12 col-sm-12" runat="server" id="divTubeSizeInfo">
                                        <a class="btn btn-primary " data-bs-toggle="offcanvas" href="#canvasInfo" role="button" aria-controls="canvasInfo" onclick="return showInfo('Tube Size');"><i class="bi bi-info-circle me-2"></i>Info</a>
                                    </div>                                    
                                </div>

                                <div class="mb-1 mt-6 row" runat="server" id="divAdditional">
                                    <p style="color:red;">
                                        <b><u>ADDITIONAL</u></b>
                                    </p>
                                </div>

                                <div class="mb-3 row" runat="server" id="divChildSafe">
                                    <label class="col-lg-3 col-form-label">CHILDSAFE</label>
                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlChildSafe" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Clear Loop (Standard)" Text="CLEAR LOOP (STANDARD)"></asp:ListItem>
                                            <asp:ListItem Value="Black - Deluxe" Text="BLACK - DELUXE"></asp:ListItem>
                                            <asp:ListItem Value="Grey - Deluxe" Text="GREY - DELUXE"></asp:ListItem>
                                            <asp:ListItem Value="Birch White - Deluxe" Text="BIRCH WHITE - DELUXE"></asp:ListItem>
                                            <asp:ListItem Value="White - Deluxe" Text="WHITE - DELUXE"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-3 row" runat="server" id="divAccessory">
                                    <label class="col-lg-3 col-form-label">ACCESSORY</label>
                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlAccessory" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Crochet Ring Pull" Text="CROCHET RING PULL"></asp:ListItem>
                                            <asp:ListItem Value="Tassle Pull" Text="TASSLE PULL"></asp:ListItem>
                                            <asp:ListItem Value="Silver Metal Ring" Text="SILVER METAL RING"></asp:ListItem>
                                            <asp:ListItem Value="Gold Metal Ring" Text="GOLD METAL RING"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-3 row" runat="server" id="divExtras">
                                    <label class="col-lg-3 col-form-label">EXTRAS</label>
                                    <div class="col-lg-5 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlExtras" CssClass="form-select"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-3 mt-6 row" runat="server" id="divBracketCover">
                                    <label class="col-lg-3 col-form-label">BRACKET COVERS</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlBracketCover" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Yes" Text="YES"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-3 row" runat="server" id="divBracketExt">
                                    <label class="col-lg-3 col-form-label">BRACKET EXTENSION</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlBracketExt" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Yes" Text="YES"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-3 mt-6 row">
                                    <label class="col-lg-3 col-form-label">SPECIAL INFORMATION</label>
                                    <div class="col-lg-8 col-md-12 col-sm-12">
                                        <asp:TextBox runat="server" TextMode="MultiLine" ID="txtNotes" Height="100px" CssClass="form-control" placeholder="Your notes ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="mb-3 row" runat="server" id="divMarkUp">
                                    <label class="col-lg-3 col-form-label">MARK UP (%)</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:TextBox runat="server" TextMode="Number" ID="txtMarkUp" CssClass="form-control" placeholder="Mark Up ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>
                            </div>

                            <div class="row" runat="server" id="divError">
                                <div class="col-lg-12">
                                    <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                        <div class="d-flex">
                                            <div>
                                                <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                            </div>
                                            <div>
                                                <span runat="server" id="msgError"></span>
                                            </div>
                                        </div>
                                        <a class="btn-close" data-bs-dismiss="alert" aria-label="close"></a>
                                    </div>
                                </div>
                                
                            </div>
                        </div>
                        <div class="card-footer text-end">
                            <asp:LinkButton runat="server" ID="btnSubmit" CssClass="btn btn-primary " OnClick="btnSubmit_Click" ></asp:LinkButton>
                            <asp:LinkButton runat="server" ID="btnCancel"  CssClass="btn btn-danger " OnClick="btnCancel_Click" >
                                <i class="fa-solid fa-arrow-rotate-left me-2"></i>Cancel
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>

                <div class="col-lg-5 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Notes</h3>
                        </div>

                        <div class="card-body">
                            <div class="mb-3 row">
                                <p runat="server" id="pNotes"></p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="offcanvas offcanvas-start" tabindex="-1" id="canvasInfo" aria-labelledby="canvasInfoLabel">
        <div class="offcanvas-header">
            <h2 class="offcanvas-title" id="canvasInfoLabel"></h2>
            <button type="button" class="btn-close text-reset" data-bs-dismiss="offcanvas" aria-label="Close"></button>
        </div>
        <div class="offcanvas-body">
            <div class="text-body">
                <span id="spanInfo" style="font-size:large;"></span>
            </div>
            <div class="mt-3">
                <button class="btn btn-primary " type="button" data-bs-dismiss="offcanvas">
                    <i class="bi bi-check2-circle me-2" style="font-size: 12pt;"></i>Ok. I Got It
                </button>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalSuccess" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-success"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" class="icon mb-2 text-green icon-lg" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M12 12m-9 0a9 9 0 1 0 18 0a9 9 0 1 0 -18 0" /><path d="M9 12l2 2l4 -4" /></svg>
                    <h3>Succedeed</h3>
                    <div class="text-secondary">
                        This is the <b><span id="spanBlind"></span></b><br />from <b><span id="spanBlindName"></span></b> - <b><span id="spanBracketName"></span></b>
                        <br /><br />
                        Please click the <b>ADD BLIND</b> button that is written in green color of the <b>ITEM ID <span id="spanItemId"></span></b>.
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col text-end">
                                <button type="button" class="btn btn-primary" data-bs-dismiss="modal">
                                    <i class="bi bi-check2-circle me-2" style="font-size: 12pt;"></i>OK. I Got It
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- my custom script -->
    <script text="text/javascript" src="/Content/dist/js/my/roller.js"></script>
    <script type="text/javascript">

        document.addEventListener("DOMContentLoaded", () => {
            loaderFadeOut();
        })

        // Function untuk menampilkan pesan error dari code-behind
        function showPopUpWfMotorised(msg){
            Swal.fire({
                icon: "success",
                title: "Success !",
                customClass: {
                    popup: isDark ? "bg-dark text-white" : "bg-white text-dark"
                },
                html: msg,
            }).then((result) => {
                if (result.isConfirmed) {
                    window.location.href = "/order/detail";
                }
            });
        }
        function showMessageError(msg){
            Swal.fire({
                icon: "error",
                title: "Oops...",
                customClass: {
                    popup: isDark ? "bg-dark text-white" : "bg-white text-dark"
                },
                html: msg,
            });
        }


        $(function () {
            $("#modalSuccess").on('hidden.bs.modal', function (e) {
                window.location.href = "/order/detail";
            });
        });
        function showConfirm(itemId, blindNo) {
            var blindNoSelected = 'first blind';
            if (blindNo == 'Blind 2') {
                blindNoSelected = 'second blind';
            }

            var blindType = document.getElementById("<%= ddlBlindType.ClientID %>");
            var blindSelected = blindType.options[blindType.selectedIndex].innerHTML;

            var bracketType = document.getElementById("<%= ddlBracketType.ClientID %>");
            var bracketSelected = bracketType.options[bracketType.selectedIndex].innerHTML;

            document.getElementById("spanBlindName").innerHTML = blindSelected;
            document.getElementById("spanBracketName").innerHTML = bracketSelected;
            document.getElementById("spanBlind").innerHTML = blindNoSelected.toUpperCase();
            document.getElementById("spanItemId").innerHTML = itemId;

            $('#modalSuccess').modal('show');
        }

        function showInfo(Type) {
            var spanInfo;

            if (Type == 'Quantity') {
                spanInfo = 'Please pay attention to the quantity you want to order, because the quantity you enter will be processed automatically.';
            } else if (Type == 'Motor Style') {
                spanInfo = 'If any another blind (Double or linked)';
                spanInfo = '<br />';
                spanInfo = 'If you change this MOTOR STYLE then the other motor style will follow this motor style.';
            } else if (Type == 'Motor Remote') {
                spanInfo = 'If any another blind (Double or linked)';
                spanInfo = '<br />';
                spanInfo = 'If you change this MOTOR REMOTE then the other motor remote will follow this motor remote.';
            } else if (Type == 'Size') {
                var bracketType = document.getElementById("<%= ddlBracketType.ClientID %>");
                var bracketSelected = bracketType.options[bracketType.selectedIndex].innerHTML;
                spanInfo = 'If you change this drop then the previous blind will follow this drop size.';
                if (bracketSelected == 'DOUBLE') {
                    spanInfo = 'If you change this size then the previous blind will follow this size.';
                }
                spanInfo += '<br /><br />';
                spanInfo += '* If any. (System will check automatically)'
            } else if (Type == 'Trim') {
                spanInfo = 'If you want to use the BOTTOM RAIL.<br />Please select <b>1F</b>.';
            } else if (Type == 'Mounting') {
                spanInfo = '*';
            } else if (Type == 'Tube Size') {
                spanInfo = 'Our standard tube size';
                spanInfo += '<br /><br />';
                spanInfo += '1. If the width or drop are below 2400 then the tube size uses 40';
                spanInfo += '<br />';
                spanInfo += '2. If the width or drop are more than 2400 then the tube size uses 45';
                spanInfo += '<br />';
                spanInfo += '3. If the width or drop are more than 2600 then the tube size uses 45H';
            }
            document.getElementById("canvasInfoLabel").innerHTML = Type + ' Information';
            document.getElementById("spanInfo").innerHTML = spanInfo;
        }
    </script>

    <div runat="server" visible="False">
        <asp:Label runat="server" ID="lblHeaderId"></asp:Label>
        <asp:Label runat="server" ID="lblItemId"></asp:Label>

        <asp:Label runat="server" ID="lblKitId"></asp:Label>
        <asp:Label runat="server" ID="lblSoeKitId"></asp:Label>

        <asp:Label runat="server" ID="lblSkinType"></asp:Label>

        <asp:Label runat="server" ID="lblPriceGroupId"></asp:Label>
        <asp:Label runat="server" ID="lblCassetteExtraId"></asp:Label>

        <asp:Label runat="server" ID="lblChainId"></asp:Label>
        <asp:Label runat="server" ID="lblChainLength"></asp:Label>

        <asp:Label runat="server" ID="lblBlindNo"></asp:Label>
        <asp:Label runat="server" ID="lblUniqueId"></asp:Label>

        <asp:Label runat="server" ID="lblTubeSize"></asp:Label>

        <asp:Label runat="server" ID="lblTrim"></asp:Label>
        <asp:Label runat="server" ID="sesiLogin"></asp:Label>

        <asp:SqlDataSource ID="sdsNext" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE OrderDetails SET TubeSize=@TubeSize, Mounting=@Mounting, Location=@Location, ChildSafe=@ChildSafe, Accessory=@Accessory, BracketCover=@BracketCover, BracketExtension=@BracketExtension, MotorStyle=@MotorStyle, MarkUp=@MarkUp WHERE UniqueId=@UniqueId AND Active=1">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblUniqueId" Name="UniqueId" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtLocation" Name="Location" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlMounting" Name="Mounting" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlAccessory" Name="Accessory" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlTubeSize" Name="TubeSize" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlBracketCover" Name="BracketCover" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlBracketExt" Name="BracketExtension" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlChildSafe" Name="ChildSafe" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlMotorStyle" Name="MotorStyle" PropertyName="SelectedItem.Value" />
                <%-- <asp:ControlParameter ControlID="ddlMotorRemote" Name="MotorRemote" PropertyName="SelectedItem.Value" /> --%>
                <asp:ControlParameter ControlID="ddlExternalBattery" Name="MotorBattery" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlMotorCharger" Name="MotorCharger" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlConnector" Name="Connector" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlCableExitPoint" Name="CableExitPoint" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtNotes" Name="Notes" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtMarkUp" Name="MarkUp" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>
        

        <!-- this is for updating the DB Linked 2 (Dep) first -->
        <asp:SqlDataSource ID="sdsDB2Depfirst" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE OrderDetails SET FabricId=@FabricId, PriceGroupId=@PriceGroupId, RollDirection=@RollDirection WHERE BlindNo='Blind 1' Or BlindNo='Blind 2' AND UniqueId=@UniqueId AND Active=1">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblUniqueId" Name="UniqueId" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlFabricColour" Name="FabricId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblPriceGroupId" Name="PriceGroupId" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlRoll" Name="RollDirection" PropertyName="SelectedItem.Value" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <!-- this is for updating the DB Linked 2 (Ind) first -->
        <asp:SqlDataSource ID="sdsDB2Indfirst" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE OrderDetails SET FabricId=@FabricId, PriceGroupId=@PriceGroupId WHERE BlindNo='Blind 1' Or BlindNo='Blind 2' AND UniqueId=@UniqueId AND Active=1">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblUniqueId" Name="UniqueId" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlFabricColour" Name="FabricId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblPriceGroupId" Name="PriceGroupId" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <!-- this is for updating the DB Linked 2 (Dep) second -->
        <asp:SqlDataSource ID="sdsDB2Depsecond" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE OrderDetails SET FabricId=@FabricId, PriceGroupId=@PriceGroupId, RollDirection=@RollDirection WHERE BlindNo IN ('Blind 3', 'Blind 4') AND UniqueId=@UniqueId AND Active=1">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblUniqueId" Name="UniqueId" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlFabricColour" Name="FabricId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblPriceGroupId" Name="PriceGroupId" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlRoll" Name="RollDirection" PropertyName="SelectedItem.Value" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <!-- this is for updating the DB Linked 2 (Ind) second -->
        <asp:SqlDataSource ID="sdsDB2Indsecond" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE OrderDetails SET FabricId=@FabricId, PriceGroupId=@PriceGroupId WHERE BlindNo IN ('Blind 3', 'Blind 4') AND UniqueId=@UniqueId AND Active=1">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblUniqueId" Name="UniqueId" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlFabricColour" Name="FabricId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblPriceGroupId" Name="PriceGroupId" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <!-- Fabric Update -->
        <asp:SqlDataSource ID="sdsFabric" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE OrderDetails SET FabricId=@FabricId, PriceGroupId=@PriceGroupId WHERE UniqueId=@UniqueId AND Active=1">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblUniqueId" Name="UniqueId" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlFabricColour" Name="FabricId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblPriceGroupId" Name="PriceGroupId" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <!-- Size Update -->
        <asp:SqlDataSource ID="sdsSize" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE OrderDetails SET Width=@Width, [Drop]=@Drop WHERE UniqueId=@UniqueId AND Active=1">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblUniqueId" Name="UniqueId" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtWidth" Name="Width" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtDrop" Name="Drop" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>
        
        <!-- Drop Update -->
        <asp:SqlDataSource ID="sdsDrop" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE OrderDetails SET [Drop]=@Drop WHERE UniqueId=@UniqueId AND Active=1">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblUniqueId" Name="UniqueId" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtDrop" Name="Drop" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <!-- Roll Direction Update -->
        <asp:SqlDataSource ID="sdsRollDep" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE OrderDetails SET RollDirection=@RollDirection WHERE UniqueId=@UniqueId AND Active=1">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblUniqueId" Name="UniqueId" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlRoll" Name="RollDirection" PropertyName="SelectedItem.Value" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <!-- TubeSize Update -->
        <asp:SqlDataSource ID="sdsTubeSize" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE OrderDetails SET TubeSize=@TubeSize WHERE UniqueId=@UniqueId AND Active=1">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblUniqueId" Name="UniqueId" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlTubeSize" Name="TubeSize" PropertyName="SelectedItem.Value" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="sdsPage" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" InsertCommand="INSERT INTO OrderDetails(Id, HeaderId, UniqueId, BlindNo, KitId, SoeKitId, FabricId, ChainId, BottomRailId, PriceGroupId, CassetteExtraId, Qty, Location, Mounting, Width, [Drop], RollDirection, ControlPosition, ChainLength, Accessory, TubeSize, Trim, BracketCover, BracketExtension, ChildSafe, MotorStyle, MotorRemote, MotorBattery, MotorCharger, Connector, AdditionalMotor, CableExitPoint, Notes, Matrix, Charge, TotalMatrix, TotalCharge, MarkUp, Active) VALUES (@Id, @HeaderId, @UniqueId, @BlindNo, @KitId, @SoeKitId, @FabricId, @ChainId, @BottomRailId, @PriceGroupId, @CassetteExtraId, @Qty, @Location, @Mounting, @Width, @Drop, @RollDirection, @ControlPosition, @ChainLength, @Accessory, @TubeSize, @Trim, @BracketCover, @BracketExtension, @ChildSafe, @MotorStyle, @MotorRemote, @MotorBattery, @MotorCharger, @Connector, @AdditionalMotor, @CableExitPoint, @Notes, 0.00, 0.00, 0.00, 0.00, @MarkUp, 1)" UpdateCommand="UPDATE OrderDetails SET BlindNo=@BlindNo, UniqueId=@UniqueId, KitId=@KitId, SoeKitId=@SoeKitId, FabricId=@FabricId, ChainId=@ChainId, BottomRailId=@BottomRailId, PriceGroupId=@PriceGroupId, CassetteExtraId=@CassetteExtraId, Qty=@Qty, Location=@Location, Mounting=@Mounting, Width=@Width, [Drop]=@Drop, RollDirection=@RollDirection, ControlPosition=@ControlPosition, ChainLength=@ChainLength, Accessory=@Accessory, TubeSize=@TubeSize, Trim=@Trim, BracketCover=@BracketCover, BracketExtension=@BracketExtension, ChildSafe=@ChildSafe, MotorStyle=@MotorStyle, MotorRemote=@MotorRemote, MotorBattery=@MotorBattery, MotorCharger=@MotorCharger, Connector=@Connector, AdditionalMotor=@AdditionalMotor, CableExitPoint=@CableExitPoint, Notes=@Notes, MarkUp=@MarkUp, Active=1 WHERE Id=@Id">
            <InsertParameters>
                <asp:ControlParameter ControlID="lblItemId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblHeaderId" Name="HeaderId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblUniqueId" Name="UniqueId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblBlindNo" Name="BlindNo" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblKitId" Name="KitId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblSoeKitId" Name="SoeKitId" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlFabricColour" Name="FabricId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblChainId" Name="ChainId" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlRailColour" Name="BottomRailId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblPriceGroupId" Name="PriceGroupId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblCassetteExtraId" Name="CassetteExtraId" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtQty" Name="Qty" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtLocation" Name="Location" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlMounting" Name="Mounting" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtWidth" Name="Width" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtDrop" Name="Drop" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlRoll" Name="RollDirection" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlControlPosition" Name="ControlPosition" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblChainLength" Name="ChainLength" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlAccessory" Name="Accessory" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlTubeSize" Name="TubeSize" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblTrim" Name="Trim" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlBracketCover" Name="BracketCover" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlBracketExt" Name="BracketExtension" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlChildSafe" Name="ChildSafe" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlMotorStyle" Name="MotorStyle" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlMotorRemote" Name="MotorRemote" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlExternalBattery" Name="MotorBattery" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlMotorCharger" Name="MotorCharger" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlConnector" Name="Connector" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlExtras" Name="AdditionalMotor" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlCableExitPoint" Name="CableExitPoint" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtNotes" Name="Notes" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtMarkUp" Name="MarkUp" PropertyName="Text" />
            </InsertParameters>
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblItemId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblHeaderId" Name="HeaderId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblUniqueId" Name="UniqueId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblBlindNo" Name="BlindNo" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblKitId" Name="KitId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblSoeKitId" Name="SoeKitId" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlFabricColour" Name="FabricId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblChainId" Name="ChainId" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlRailColour" Name="BottomRailId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblPriceGroupId" Name="PriceGroupId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblCassetteExtraId" Name="CassetteExtraId" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtQty" Name="Qty" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtLocation" Name="Location" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlMounting" Name="Mounting" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtWidth" Name="Width" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtDrop" Name="Drop" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlRoll" Name="RollDirection" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlControlPosition" Name="ControlPosition" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblChainLength" Name="ChainLength" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlAccessory" Name="Accessory" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlTubeSize" Name="TubeSize" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblTrim" Name="Trim" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlBracketCover" Name="BracketCover" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlBracketExt" Name="BracketExtension" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlChildSafe" Name="ChildSafe" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlMotorStyle" Name="MotorStyle" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlMotorRemote" Name="MotorRemote" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlExternalBattery" Name="MotorBattery" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlMotorCharger" Name="MotorCharger" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlConnector" Name="Connector" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlExtras" Name="AdditionalMotor" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlCableExitPoint" Name="CableExitPoint" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtNotes" Name="Notes" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtMarkUp" Name="MarkUp" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>