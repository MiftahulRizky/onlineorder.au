<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Verishades.aspx.vb" Inherits="Order_Verishades" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Veri Shades Order" %>

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
                                <label class="col-lg-3 col-form-label">TYPE</label>
                                <div class="col-lg-3 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlBlindType" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlBlindType_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-3 row" runat="server" visible="false">
                                <label class="col-lg-3 col-form-label">VERI SHADES TYPE</label>
                                <div class="col-lg-3 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlKitId" CssClass="form-select" AutoPostBack="true"></asp:DropDownList>
                                </div>
                            </div>

                            <div runat="server" id="divDetail">
                                <hr />
                                <div class="mb-3 row">
                                    <label class="col-lg-3 col-form-label">BLIND QTY</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:TextBox runat="server" TextMode="Number" ID="txtQty" CssClass="form-control" placeholder="Blind Qty ..." autocomplete="off"></asp:TextBox>
                                    </div>

                                    <div class="col-lg-2 col-sm-12">
                                        <a class="btn btn-primary" data-bs-toggle="offcanvas" href="#canvasInfo" role="button" aria-controls="canvasInfo" onclick="return showInfo('Quantity');"><i class="bi bi-info-circle me-2"></i>Info</a>
                                    </div>
                                </div>

                                <div class="mb-3 row">
                                    <label class="col-lg-3 col-form-label">ROOM TO INSTALL</label>
                                    <div class="col-lg-6 col-md-12 col-sm-12">
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

                                <div class="mb-3 mt-6 row" runat="server" id="divFabric">
                                    <label class="col-lg-3 col-form-label">FABRIC</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlFabricType" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlFabricType_SelectedIndexChanged"></asp:DropDownList>
                                        <small class="form-hint">* Type</small>
                                    </div>

                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlFabricColour" CssClass="form-select"></asp:DropDownList>
                                        <small class="form-hint">* Colour</small>
                                    </div>
                                </div>

                                <div class="mb-3 mt-6 row">
                                    <asp:Label runat="server" CssClass="col-md-3 col-form-label" ID="lblSize" Text="WIDTH x DROP"></asp:Label>
                                    <div class="col-lg-2 col-md-12 col-sm-12" runat="server" id="divWidth">
                                        <asp:TextBox runat="server" TextMode="Number" min="1" ID="txtWidth" CssClass="form-control" placeholder="Width ..."></asp:TextBox>
                                        <small class="form-hint">* Width</small>
                                    </div>

                                    <div class="col-lg-2 col-md-12 col-sm-12" runat="server" id="divDrop">
                                        <asp:TextBox runat="server" TextMode="Number" min="1" ID="txtDrop" CssClass="form-control" placeholder="Drop ..."></asp:TextBox>
                                        <small class="form-hint">* Drop</small>
                                    </div>
                                </div>

                                <div class="mb-3 row" runat="server" id="divBlindSize">
                                    <label class="col-lg-3 col-form-label">BLIND SIZE</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlBlindSize" CssClass="form-select">
                                            <asp:ListItem Value="0" Text=""></asp:ListItem>
                                            <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                
                                <div class="mb-6 mt-6 row" runat="server" id="divStack">
                                    <label class="col-lg-3 col-form-label">STACK CONFIGURATION</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlStackPosition" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Left" Text="LEFT"></asp:ListItem>
                                            <asp:ListItem Value="Right" Text="RIGHT"></asp:ListItem>
                                            <asp:ListItem Value="Centre Stack" Text="CENTRE STACK"></asp:ListItem>
                                            <asp:ListItem Value="Centre Split" Text="CENTRE SPLIT"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                
                                <div class="mb-3 row" runat="server" id="divTrack">
                                    <label class="col-lg-3 col-form-label">TRACK TYPE - COLOUR</label>
                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlTrackType" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Cube" Text="CUBE"></asp:ListItem>
                                            <asp:ListItem Value="Decorative (Flat)" Text="DECORATIVE (FLAT)"></asp:ListItem>
                                            <asp:ListItem Value="Decorative (Round)" Text="DECORATIVE (ROUND)"></asp:ListItem>
                                            <asp:ListItem Value="Standard" Text="STANDARD"></asp:ListItem>
                                        </asp:DropDownList>
                                        <small class="form-hint">* Track Type</small>
                                    </div>

                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlTrackColour" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Birch" Text="BIRCH"></asp:ListItem>
                                            <asp:ListItem Value="Black" Text="BLACK"></asp:ListItem>
                                            <asp:ListItem Value="White" Text="WHITE"></asp:ListItem>
                                        </asp:DropDownList>
                                        <small class="form-hint">* Track Colour</small>
                                    </div>
                                </div>
                                
                                <div class="mb-3 mt-6 row" runat="server" id="divWand">
                                    <label class="col-lg-3 col-form-label">WAND SIZE x COLOUR</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlWandSize" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlWandSize_SelectedIndexChanged"></asp:DropDownList>
                                        <small class="form-hint">* Size</small>
                                    </div>

                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlWandColour" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlWandColour_SelectedIndexChanged"></asp:DropDownList>
                                        <small class="form-hint">* Colour</small>
                                    </div>

                                    <div class="col-lg-2 col-md-12 col-sm-12" runat="server" id="divWandCustomLength">
                                        <asp:TextBox runat="server" TextMode="Number" ID="txtWandCustomLength" CssClass="form-control" placeholder="Custom..." autocomplete="off"></asp:TextBox>
                                        <small class="form-hint">* Custom</small>
                                    </div>

                                    <div class="col-lg-2 col-md-12 col-sm-12" runat="server" id="divBtnInfoCustom">
                                        <a class="btn btn-primary" data-bs-toggle="offcanvas" href="#canvasInfo" role="button" aria-controls="canvasInfo" onclick="return showInfo('Custom Wand Length');"><i class="bi bi-info-circle me-2"></i>Info</a>
                                    </div>

                                </div>

                                <div class="mb-3 mt-6 row">
                                    <label class="col-lg-3 col-form-label">SPECIAL INFORMATION</label>
                                    <div class="col-lg-8 col-md-12 col-sm-12">
                                        <asp:TextBox runat="server" TextMode="MultiLine" ID="txtNotes" Height="100px" CssClass="form-control" placeholder="Your notes ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="mb-3 row" runat="server" id="divMarkUp">
                                    <label class="col-lg-3 col-form-label" >MARK UP (%)</label>
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
                            <asp:LinkButton runat="server" ID="btnSubmit" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmit_Click" ></asp:LinkButton>
                            <asp:LinkButton runat="server" ID="btnCancel" Text="Cancel" CssClass="btn btn-danger" OnClick="btnCancel_Click" >
                                <i class="fa-solid fa-rotate-left me-2"></i> Cancel
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>

                <div class="col-lg-5 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Notes</h3>
                        </div>

                        <div class="card-body"></div>
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
                <button class="btn btn-primary" type="button" data-bs-dismiss="offcanvas">
                    <i class="bi bi-check2-circle me-2" style="font-size: 12pt;"></i>Ok. I Got It
                </button>
            </div>
        </div>
    </div>


    <!-- my custom script -->
     <script type="text/javascript" src="/Content/dist/js/my/verishades.js"></script>
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
                spanInfo = 'Maximum custom length 3000mm';
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

        <asp:Label runat="server" ID="lblPriceGroupId"></asp:Label>
        <asp:Label runat="server" ID="lblWandSize"></asp:Label>

        <asp:SqlDataSource ID="sdsPage" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" InsertCommand="INSERT INTO OrderDetails(Id, HeaderId, KitId, SoeKitId, FabricId, PriceGroupId, BlindNo, Qty, BlindSize, Location, Mounting, Width, [Drop], StackPosition, TrackType, TrackColour, WandLength, WandColour, Notes, Matrix, Charge, TotalMatrix, TotalCharge, MarkUp, Active) VALUES(@Id, @HeaderId, @KitId, @SoeKitId, @FabricId, @PriceGroupId, 'Blind 1', @Qty, @BlindSize, @Location, @Mounting, @Width, @Drop, @StackPosition, @TrackType, @TrackColour, @WandLength, @WandColour, @Notes, 0.00, 0.00, 0.00, 0.00, @MarkUp, 1)" UpdateCommand="UPDATE OrderDetails SET KitId=@KitId, SoeKitId=@SoeKitId, FabricId=@FabricId, PriceGroupId=@PriceGroupId, BlindNo='Blind 1', Qty=@Qty, BlindSize=@BlindSize, Location=@Location, Mounting=@Mounting, Width=@Width, [Drop]=@Drop, StackPosition=@StackPosition, TrackType=@TrackType, TrackColour=@TrackColour, WandLength=@WandLength, WandColour=@WandColour, Notes=@Notes, MarkUp=@MarkUp, Active=1 WHERE Id=@Id">
            <InsertParameters>
                <asp:ControlParameter ControlID="lblItemId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblHeaderId" Name="HeaderId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblKitId" Name="KitId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblSoeKitId" Name="SoeKitId" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlFabricColour" Name="FabricId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblPriceGroupId" Name="PriceGroupId" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlBlindSize" Name="BlindSize" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtQty" Name="Qty" PropertyName="Text" />                
                <asp:ControlParameter ControlID="ddlMounting" Name="Mounting" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtLocation" Name="Location" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtWidth" Name="Width" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtDrop" Name="Drop" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlStackPosition" Name="StackPosition" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlTrackType" Name="TrackType" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlTrackColour" Name="TrackColour" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblWandSize" Name="WandLength" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlWandColour" Name="WandColour" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtNotes" Name="Notes" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtMarkUp" Name="MarkUp" PropertyName="Text" />
            </InsertParameters>
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblItemId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblHeaderId" Name="HeaderId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblKitId" Name="KitId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblSoeKitId" Name="SoeKitId" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlFabricColour" Name="FabricId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblPriceGroupId" Name="PriceGroupId" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlBlindSize" Name="BlindSize" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtQty" Name="Qty" PropertyName="Text" />                
                <asp:ControlParameter ControlID="ddlMounting" Name="Mounting" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtLocation" Name="Location" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtWidth" Name="Width" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtDrop" Name="Drop" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlStackPosition" Name="StackPosition" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlTrackType" Name="TrackType" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlTrackColour" Name="TrackColour" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblWandSize" Name="WandLength" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlWandColour" Name="WandColour" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtNotes" Name="Notes" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtMarkUp" Name="MarkUp" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>