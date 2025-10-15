<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Vertical.aspx.vb" Inherits="Order_Vertical" MaintainScrollPositionOnPostback="true" MasterPageFile="~/Site.master" Debug="true" Title="Vertical Order" %>

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
                        </div>
                        <div class="card-body">
                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">VERTICAL TYPE</label>
                                <div class="col-lg-3 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlBlindType" CssClass="form-select" Font-Bold="true" AutoPostBack="true" OnSelectedIndexChanged="ddlBlindType_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-3 row" runat="server" id="divVerticalStyle">
                                <asp:Label runat="server" ID="lblVerticalStyle" CssClass="col-lg-3 col-form-label" Text="VERTICAL STYLE"></asp:Label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlTubeType" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlTubeType_SelectedIndexChanged" Font-Bold="true"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-3 row" runat="server" id="divControlType">
                                <label class="col-lg-3 col-form-label">CONTROL TYPE</label>
                                <div class="col-lg-3 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlControlType" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlControlType_SelectedIndexChanged" Font-Bold="true"></asp:DropDownList>
                                </div>
                            </div>

                            <div runat="server" id="divDetail">
                                <hr />
                                <div class="mb-3 row">
                                    <label class="col-lg-3 col-form-label">QUANTITY</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:TextBox runat="server" TextMode="Number" ID="txtQty" CssClass="form-control" placeholder="Qty ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <a class="btn btn-primary " data-bs-toggle="offcanvas" href="#canvasInfo" role="button" aria-controls="canvasInfo" onclick="return showInfo('Quantity');"><i class="bi bi-info-circle me-2"></i>Info</a>
                                    </div>
                                </div>

                                <div class="mb-3 row">
                                    <label class="col-lg-3 col-form-label">ROOM TO INSTALL</label>
                                    <div class="col-lg-5 col-md-12 col-sm-12">
                                        <asp:TextBox runat="server" ID="txtLocation" CssClass="form-control" placeholder="Location ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="mb-3 row" runat="server" id="divMounting">
                                    <label class="col-lg-3 col-form-label">MOUNTING</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlMounting" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Face Fit" Text="FACE FIT"></asp:ListItem>
                                            <asp:ListItem Value="Reveal Fit" Text="REVEAL FIT"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-3 mt-6 row" runat="server" id="divSlatSize">
                                    <label class="col-lg-3 col-form-label">SLAT SIZE</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlSlatSize" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="127mm" Text="127mm"></asp:ListItem>
                                            <asp:ListItem Value="100mm" Text="100mm"></asp:ListItem>
                                            <asp:ListItem Value="89mm" Text="89mm"></asp:ListItem>
                                            <asp:ListItem Value="63mm" Text="63mm"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-3 row" runat="server" id="divSlatQty">
                                    <label class="col-lg-3 col-form-label">SLAT QTY</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:TextBox runat="server" TextMode="Number" ID="txtSlatQty" CssClass="form-control" placeholder="Slat Qty ..." autocomplete="off"></asp:TextBox>
                                    </div>

                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <a class="btn btn-primary " data-bs-toggle="offcanvas" href="#canvasInfo" role="button" aria-controls="canvasInfo" onclick="return showInfo('Slat QTY');"><i class="bi bi-info-circle me-2"></i>Info</a>
                                    </div>
                                </div>

                                <div class="mb-3 mt-6 row" runat="server" id="divFabricType">
                                    <label class="col-lg-3 col-form-label">FABRIC TYPE</label>
                                    <div class="col-lg-6 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlFabricType" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlFabricType_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-3 row" runat="server" id="divFabricLength">
                                    <label class="col-lg-3 col-form-label text-uppercase">Fabric/Slat Size</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlFabricLength" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlFabricLength_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-3 row" runat="server" id="divFabricColour">
                                    <label class="col-lg-3 col-form-label">FABRIC COLOUR</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlFabricColour" CssClass="form-select"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-6 mt-5 row">
                                    <asp:Label runat="server" CssClass="col-lg-3 col-form-label" ID="lblSize" Text="WIDTH x DROP"></asp:Label>
                                    <div class="col-lg-2 col-md-12 col-sm-12" runat="server" id="divWidth">
                                        <asp:TextBox runat="server" TextMode="Number" min="1" ID="txtWidth" CssClass="form-control" placeholder="Width ..." autocomplete="off"></asp:TextBox>
                                        <small class="form-hint">* Width</small>
                                    </div>

                                    <div class="col-lg-2 col-md-12 col-sm-12" runat="server" id="divDrop">
                                        <asp:TextBox runat="server" TextMode="Number" min="1" ID="txtDrop" CssClass="form-control" placeholder="Drop ..." autocomplete="off"></asp:TextBox>
                                        <small class="form-hint">* Drop</small>
                                    </div>

                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <a class="btn btn-primary " data-bs-toggle="offcanvas" href="#canvasInfo" role="button" aria-controls="canvasInfo" onclick="return showInfo('Width x Drop');"><i class="bi bi-info-circle me-2"></i>Info</a>
                                    </div>
                                </div>

                                <div class="mb-3 row" runat="server" id="divTrackColour">
                                    <label class="col-lg-3 col-form-label">TRACK COLOUR</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlTrackColour" CssClass="form-select"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-3 row" runat="server" id="divStackPosition">
                                    <label class="col-lg-3 col-form-label">STACK POSITION</label>
                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlStackPosition" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Left" Text="LEFT"></asp:ListItem>
                                            <asp:ListItem Value="Right" Text="RIGHT"></asp:ListItem>
                                            <asp:ListItem Value="Center" Text="CENTER"></asp:ListItem>
                                            <asp:ListItem Value="Split / Centre Open" Text="SPLIT / CENTRE OPEN"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                
                                <div class="mb-3 mt-6 row" runat="server" id="divControlPosition">
                                    <label class="col-lg-3 col-form-label">CONTROL POSITION</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlControlPosition" CssClass="form-select"></asp:DropDownList>
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
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <asp:TextBox runat="server" TextMode="Number" ID="txtChainLength" CssClass="form-control" placeholder="Chain Length ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="mb-3 row" runat="server" id="divWand">
                                    <label class="col-lg-3 col-form-label">WAND SIZE x COLOUR</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlWandLength" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlWandLength_SelectedIndexChanged"></asp:DropDownList>
                                        <small class="form-hint">* Length</small>
                                    </div>

                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlWandColour" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlWandColour_SelectedIndexChanged"></asp:DropDownList>
                                        <small class="form-hint">* Colour</small>
                                    </div>


                                    <div class="col-lg-2 col-md-12 col-sm-12" runat="server" id="divWandCustomLength">
                                        <asp:TextBox runat="server" TextMode="Number" ID="txtWandCustomLength" CssClass="form-control" placeholder="Custom..." autocomplete="off"></asp:TextBox>
                                        <small class="form-hint">* Custom Length</small>
                                    </div>

                                    <div class="col-lg-2 col-md-12 col-sm-12" runat="server" id="divBtnInfoCustom">
                                        <a class="btn btn-primary " data-bs-toggle="offcanvas" href="#canvasInfo" role="button" aria-controls="canvasInfo" onclick="return showInfo('Custom Wand Length');"><i class="bi bi-info-circle me-2"></i>Info</a>
                                    </div>
                                </div>                                

                                <div class="mb-3 mt-6 row" runat="server" id="divBrackets">
                                    <label class="col-lg-3 col-form-label">BRACKETS</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlBrackets" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="127mm F/Fit" Text="127MM F/FIT"></asp:ListItem>
                                            <asp:ListItem Value="100mm F/Fit" Text="100MM F/FIT"></asp:ListItem>
                                            <asp:ListItem Value="89mm F/Fit" Text="89MM F/FIT"></asp:ListItem>
                                            <asp:ListItem Value="C/Fit" Text="C/FIT"></asp:ListItem>
                                            <asp:ListItem Value="Ext F/Fit" Text="EXT F/FIT"></asp:ListItem>
                                            <asp:ListItem Value="Ext C/Fit" Text="EXT C/FIT"></asp:ListItem>
                                            <asp:ListItem Value="Ext" Text="EXT"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-6 row" runat="server" id="divBracketColour">
                                    <label class="col-lg-3 col-form-label">BRACKET COLOUR</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlBracketColour" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Silver" Text="SILVER"></asp:ListItem>
                                            <asp:ListItem Value="White" Text="WHITE"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-3 row" runat="server" id="divHangerType">
                                    <label class="col-lg-3 col-form-label">HANGER TYPE</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlHangerType" CssClass="form-select"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-3 row" runat="server" id="divBottom">
                                    <label class="col-lg-3 col-form-label">BOTTOM</label>
                                    <div class="col-lg-5 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlBottom" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem hidden Value="Chained (Black)" Text="CHAINED (BLACK)"></asp:ListItem>
                                            <asp:ListItem Value="Chained (White)" Text="CHAINED (WHITE)"></asp:ListItem>
                                            <asp:ListItem Value="Fully Sewn In" Text="FULLY SEWN IN"></asp:ListItem>
                                            <asp:ListItem Value="Plastic Chainless" Text="PLASTIC CHAINLESS"></asp:ListItem>
                                            <asp:ListItem hidden Value="Plastic Chainless (Black)" Text="PLASTIC CHAINLESS (BLACK)"></asp:ListItem>
                                            <asp:ListItem Value="Plastic Chainless (White)" Text="PLASTIC CHAINLESS (WHITE)"></asp:ListItem>
                                            <asp:ListItem Value="Top Hanger Only" Text="TOP HANGER ONLY"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-3 row" runat="server" id="divInsertInTrack">
                                    <label class="col-lg-3 col-form-label">INSERT IN TRACK</label>
                                    <div class="col-lg-9 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlInsertInTrack" CssClass="form-select" Width="100px">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                                        </asp:DropDownList>
                                        <small runat="server" id="noteInsertInTrack" class="form-hint"></small>
                                    </div>
                                </div>

                                <div class="mb-3 row" runat="server" id="divSloper">
                                    <label class="col-lg-3 col-form-label">SLOPER</label>
                                    <div class="col-lg-9 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlSloper" CssClass="form-select" Width="100px">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                                        </asp:DropDownList>

                                        <small runat="server" id="noteSloper" class="form-hint"></small>
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
                            <asp:LinkButton runat="server" ID="btnSubmit" Text="Submit" CssClass="btn btn-primary " OnClick="btnSubmit_Click" ></asp:LinkButton>
                            <asp:LinkButton runat="server" ID="btnCancel" Text="Cancel" CssClass="btn btn-danger " OnClick="btnCancel_Click" >
                                <i class="fa-solid fa-rotate-left me-2"></i>Cancel
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
            <h2 class="offcanvas-title" id="canvasInfoLabel">Start offcanvas</h2>
            <button type="button" class="btn-close text-reset" data-bs-dismiss="offcanvas" aria-label="Close"></button>
        </div>
        <div class="offcanvas-body">
            <div class="text-body">
                <span id="spanInfo" style="font-size:large;"></span>
            </div>
            <div class="mt-3">
                <button class="btn btn-primary " type="button" data-bs-dismiss="offcanvas">
                    <i class="bi bi-check2-circle me-2"></i>Ok. I Got It
                </button>
            </div>
        </div>
    </div>


    <!-- my custom script -->
     <script text="text/javascript" src="https://onlineorder.au/Content/dist/js/my/vertical.js"></script>
    <script type="text/javascript">
        document.addEventListener("DOMContentLoaded", () => {
            loaderFadeOut();
        })

        
        // Function untuk menampilkan pesan error dari code-behind
        function showMessageError(msg){
            Swal.fire({
                icon: "error",
                title: "Oops...",
                html: msg,
                customClass: {
                    popup: isDark ? "bg-dark text-white" : "bg-white text-dark"
                }
            });
        }
        function showInfo(Type) {
            var spanInfo;

            if (Type == 'Quantity') {
                spanInfo = 'Please pay attention to the quantity you want to order, because the quantity you enter will be processed automatically.';
            }else if (Type == 'Custom Wand Length') {
                spanInfo = 'Custom wand length is available in white color only with maximum length 3000mm.';
            }else if (Type == 'Width x Drop') {
                spanInfo = 'Very long tracks are not recommended. <br /> Butting shorter tracks will work more effectively.';
            }else if (Type = 'Slat QTY'){
                spanInfo = 'If left blank, the system will calculate it.';
            } else {
                spanInfo = '';
            }
            document.getElementById("canvasInfoLabel").innerHTML = Type + ' Information';
            document.getElementById("spanInfo").innerHTML = spanInfo;
        }
    </script>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblItemId"></asp:Label>
        <asp:Label runat="server" ID="lblHeaderId"></asp:Label>
        
        <asp:Label runat="server" ID="lblKitId"></asp:Label>
        <asp:Label runat="server" ID="lblSoeKitId"></asp:Label>
        <asp:Label runat="server" ID="lblBlindNo"></asp:Label>

        <asp:Label runat="server" ID="lblChainId"></asp:Label>

        <asp:Label runat="server" ID="lblPriceGroupId"></asp:Label>
        <asp:Label runat="server" ID="lblWandLength"></asp:Label>

        <!-- ====================Track & Slat Only  -->
        <asp:SqlDataSource ID="sdsNoComplate" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" InsertCommand="INSERT INTO OrderDetails(Id, HeaderId, BlindNo, KitId, SoeKitId, FabricId, ChainId, PriceGroupId, Qty, Location, Mounting, SlatSize, SlatQty, Width, [Drop], StackPosition, ControlPosition, TrackColour, ChainLength, WandColour, WandLength, BracketOption, BracketColour, HangerType, BottomHoldDown, InsertInTrack, Sloper, Notes, Matrix, Charge, TotalMatrix, TotalCharge, MarkUp, Active) VALUES (@Id, @HeaderId, @BlindNo, @KitId, @SoeKitId, @FabricId, @ChainId, @PriceGroupId, @Qty, @Location, @Mounting, @SlatSize, @SlatQty, @Width, @Drop, @StackPosition, @ControlPosition, @TrackColour, @ChainLength, @WandColour, @WandLength, @BracketOption, @BracketColour, @HangerType, @BottomHoldDown, @InsertInTrack, @Sloper, @Notes, 0.00, 0.00, 0.00, 0.00, @MarkUp, 1)" UpdateCommand="UPDATE OrderDetails SET BlindNo=@BlindNo, KitId=@KitId, SoeKitId=@SoeKitId, FabricId=@FabricId, ChainId=@ChainId, PriceGroupId=@PriceGroupId, Qty=@Qty, Location=@Location, Mounting=@Mounting, SlatSize=@SlatSize, SlatQty=@SlatQty, Width=@Width, [Drop]=@Drop, StackPosition=@StackPosition, ControlPosition=@ControlPosition, TrackColour=@TrackColour, ChainLength=@ChainLength, WandColour=@WandColour, WandLength=@WandLength, BracketOption=@BracketOption, BracketColour=@BracketColour, HangerType=@HangerType, BottomHoldDown=@BottomHoldDown, InsertInTrack=@InsertInTrack, Sloper=@Sloper, Notes=@Notes, Matrix=0.00, Charge=0.00, TotalMatrix=0.00, TotalCharge=0.00, MarkUp=@MarkUp WHERE Id=@Id">
            <InsertParameters>
                <asp:ControlParameter ControlID="lblItemId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblHeaderId" Name="HeaderId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblBlindNo" Name="BlindNo" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblKitId" Name="KitId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblSoeKitId" Name="SoeKitId" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlFabricColour" Name="FabricId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblChainId" Name="ChainId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblPriceGroupId" Name="PriceGroupId" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtQty" Name="Qty" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtLocation" Name="Location" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlMounting" Name="Mounting" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlSlatSize" Name="SlatSize" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtSlatQty" Name="SlatQty" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtWidth" Name="Width" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtDrop" Name="Drop" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlStackPosition" Name="StackPosition" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlControlPosition" Name="ControlPosition" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlTrackColour" Name="TrackColour" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtChainLength" Name="ChainLength" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlWandColour" Name="WandColour" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblWandLength" Name="WandLength" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlBrackets" Name="BracketOption" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlBracketColour" Name="BracketColour" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlHangerType" Name="HangerType" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlBottom" Name="BottomHoldDown" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlInsertInTrack" Name="InsertInTrack" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlSloper" Name="Sloper" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtNotes" Name="Notes" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtMarkUp" Name="MarkUp" PropertyName="Text" />
            </InsertParameters>
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblItemId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblHeaderId" Name="HeaderId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblBlindNo" Name="BlindNo" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblKitId" Name="KitId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblSoeKitId" Name="SoeKitId" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlFabricColour" Name="FabricId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblChainId" Name="ChainId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblPriceGroupId" Name="PriceGroupId" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtQty" Name="Qty" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtLocation" Name="Location" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlMounting" Name="Mounting" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlSlatSize" Name="SlatSize" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtSlatQty" Name="SlatQty" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtWidth" Name="Width" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtDrop" Name="Drop" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlStackPosition" Name="StackPosition" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlControlPosition" Name="ControlPosition" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlTrackColour" Name="TrackColour" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtChainLength" Name="ChainLength" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlWandColour" Name="WandColour" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblWandLength" Name="WandLength" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlBrackets" Name="BracketOption" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlBracketColour" Name="BracketColour" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlHangerType" Name="HangerType" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlBottom" Name="BottomHoldDown" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlInsertInTrack" Name="InsertInTrack" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlSloper" Name="Sloper" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtNotes" Name="Notes" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtMarkUp" Name="MarkUp" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>


        <!-- ==========Complate========== -->
        <asp:SqlDataSource ID="sdsComplete" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" InsertCommand="INSERT INTO OrderDetails(Id, HeaderId, BlindNo, KitId, SoeKitId, FabricId, ChainId, PriceGroupId, Qty, Location, Mounting, SlatSize, SlatQty, Width, [Drop], StackPosition, ControlPosition, TrackColour, ChainLength, WandColour, WandLength, BracketOption, BracketColour, HangerType, BottomHoldDown, InsertInTrack, Sloper, Notes, Matrix, Charge, TotalMatrix, TotalCharge, MarkUp, Active) VALUES (@Id, @HeaderId, @BlindNo, @KitId, @SoeKitId, @FabricId, @ChainId, @PriceGroupId, @Qty, @Location, @Mounting, @SlatSize, @SlatQty, @Width, @Drop, @StackPosition, @ControlPosition, @TrackColour, @ChainLength, @WandColour, @WandLength, @BracketOption, @BracketColour, @HangerType, @BottomHoldDown, @InsertInTrack, @Sloper, @Notes, 0.00, 0.00, 0.00, 0.00, @MarkUp, 1)" UpdateCommand="UPDATE OrderDetails SET BlindNo=@BlindNo, KitId=@KitId, SoeKitId=@SoeKitId, FabricId=@FabricId, ChainId=@ChainId, PriceGroupId=@PriceGroupId, Qty=@Qty, Location=@Location, Mounting=@Mounting, SlatSize=@SlatSize, SlatQty=@SlatQty, Width=@Width, [Drop]=@Drop, StackPosition=@StackPosition, ControlPosition=@ControlPosition, TrackColour=@TrackColour, ChainLength=@ChainLength, WandColour=@WandColour, WandLength=@WandLength, BracketOption=@BracketOption, BracketColour=@BracketColour, HangerType=@HangerType, BottomHoldDown=@BottomHoldDown, InsertInTrack=@InsertInTrack, Sloper=@Sloper, Notes=@Notes, Matrix=0.00, Charge=0.00, TotalMatrix=0.00, TotalCharge=0.00, MarkUp=@MarkUp WHERE Id=@Id">
            <InsertParameters>
                <asp:ControlParameter ControlID="lblItemId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblHeaderId" Name="HeaderId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblBlindNo" Name="BlindNo" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblKitId" Name="KitId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblSoeKitId" Name="SoeKitId" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlFabricColour" Name="FabricId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblChainId" Name="ChainId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblPriceGroupId" Name="PriceGroupId" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtQty" Name="Qty" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtLocation" Name="Location" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlMounting" Name="Mounting" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlFabricLength" Name="SlatSize" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtSlatQty" Name="SlatQty" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtWidth" Name="Width" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtDrop" Name="Drop" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlStackPosition" Name="StackPosition" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlControlPosition" Name="ControlPosition" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlTrackColour" Name="TrackColour" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtChainLength" Name="ChainLength" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlWandColour" Name="WandColour" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblWandLength" Name="WandLength" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlBrackets" Name="BracketOption" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlBracketColour" Name="BracketColour" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlHangerType" Name="HangerType" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlBottom" Name="BottomHoldDown" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlInsertInTrack" Name="InsertInTrack" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlSloper" Name="Sloper" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtNotes" Name="Notes" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtMarkUp" Name="MarkUp" PropertyName="Text" />
            </InsertParameters>
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblItemId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblHeaderId" Name="HeaderId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblBlindNo" Name="BlindNo" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblKitId" Name="KitId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblSoeKitId" Name="SoeKitId" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlFabricColour" Name="FabricId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblChainId" Name="ChainId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblPriceGroupId" Name="PriceGroupId" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtQty" Name="Qty" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtLocation" Name="Location" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlMounting" Name="Mounting" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlFabricLength" Name="SlatSize" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtSlatQty" Name="SlatQty" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtWidth" Name="Width" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtDrop" Name="Drop" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlStackPosition" Name="StackPosition" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlControlPosition" Name="ControlPosition" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlTrackColour" Name="TrackColour" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtChainLength" Name="ChainLength" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlWandColour" Name="WandColour" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblWandLength" Name="WandLength" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlBrackets" Name="BracketOption" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlBracketColour" Name="BracketColour" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlHangerType" Name="HangerType" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlBottom" Name="BottomHoldDown" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlInsertInTrack" Name="InsertInTrack" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlSloper" Name="Sloper" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtNotes" Name="Notes" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtMarkUp" Name="MarkUp" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>