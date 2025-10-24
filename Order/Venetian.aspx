<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Venetian.aspx.vb" Inherits="Order_Venetian" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Venetian Order" %>

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
                                <label class="col-lg-3 col-form-label">VENETIAN TYPE</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlBlindType" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlBlindType_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-3 row" runat="server" id="divStyle">
                                <label class="col-lg-3 col-form-label">STYLE</label>
                                <div class="col-lg-3 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlStyle" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlStyle_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">COLOUR</label>
                                <div class="col-lg-3 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlColour" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlColour_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                            </div>

                            <div runat="server" id="divDetail">
                                <hr />
                                <div class="mb-3 row">
                                    <label class="col-lg-3 col-form-label">QUANTITY</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:TextBox runat="server" TextMode="Number" ID="txtQty" CssClass="form-control" placeholder="Qty ....." autocomplete="off"></asp:TextBox>
                                    </div>
                                     <div class="col-lg-2 col-md-12 col-sm-12">
                                         <a class="btn btn-primary " data-bs-toggle="offcanvas" href="#canvasInfo" role="button" aria-controls="canvasInfo" onclick="return showInfo('Quantity');"><i class="bi bi-info-circle me-2"></i>Info</a>
                                    </div>
                                </div>

                                <div class="mb-3 row">
                                    <label class="col-lg-3 col-form-label">ROOM TO INSTALL</label>
                                    <div class="col-lg-6 col-md-12 col-sm-12">
                                        <asp:TextBox runat="server" ID="txtLocation" CssClass="form-control" placeholder="Location ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="mb-6 row">
                                    <label class="col-lg-3 col-form-label">MOUNTING</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlMounting" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Face Fit" Text="FACE FIT"></asp:ListItem>
                                            <asp:ListItem Value="Reveal Fit" Text="REVEAL FIT"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-6 row">
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

                                <div class="mb-5 row" runat="server">
                                    <label class="col-lg-3 col-form-label">CONTROL</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlControl" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="LHC" Text="LHC"></asp:ListItem>
                                            <asp:ListItem Value="RHC" Text="RHC"></asp:ListItem>
                                        </asp:DropDownList>
                                        <small class="form-hint">* Position</small>
                                    </div>

                                    <div class="col-lg-2 col-md-12 vcol-sm-12" runat="server">
                                        <asp:TextBox runat="server" TextMode="Number" ID="txtControlLength" CssClass="form-control" placeholder="Length ..." autocomplete="off"></asp:TextBox>
                                        <small class="form-hint">* Length</small>
                                    </div>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                         <a class="btn btn-primary " data-bs-toggle="offcanvas" href="#canvasInfo" role="button" aria-controls="canvasInfo" onclick="return showInfo('Control Length');"><i class="bi bi-info-circle me-2"></i>Info</a>
                                    </div>
                                </div>

                                <div class="mb-5 row">
                                    <label class="col-lg-3 col-form-label">BOTTOM HOLD DOWN</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlHoldDown" CssClass="form-select">
                                            <asp:ListItem Value="" Text="N/A"></asp:ListItem>
                                            <asp:ListItem Value="Gold" Text="GOLD"></asp:ListItem>
                                            <asp:ListItem Value="Silver" Text="SILVER"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-1 mt-6 row">
                                    <p style="color:red;">
                                        <b><u>PELMET DETAILS</u></b>
                                    </p>
                                </div>

                                <%--PELMET DETAILS--%>
                                <div class="mb-3 row">
                                    <label class="col-lg-3 col-form-label">PELMET TYPE</label>
                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlPelmetType" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlPelmetType_SelectedIndexChanged">
                                            <asp:ListItem Value="No Return" Text="NO RETURN"></asp:ListItem>
                                            <asp:ListItem Value="With Return" Text="WITH RETURN"></asp:ListItem>
                                            <asp:ListItem Value="Bay Left" Text="BAY LEFT"></asp:ListItem>
                                            <asp:ListItem Value="Bay Right" Text="BAY RIGHT"></asp:ListItem>
                                            <asp:ListItem Value="Main Bay" Text="MAIN BAY"></asp:ListItem>
                                            <asp:ListItem Value="Common" Text="COMMON"></asp:ListItem>
                                            <asp:ListItem Value="Single Left Return" Text="SINGLE LEFT RETURN"></asp:ListItem>
                                            <asp:ListItem Value="Single Right Return" Text="SINGLE RIGHT RETURN"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="mb-3 row">
                                    <label class="col-lg-3 col-form-label">PELMET SIZE</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:DropDownList runat="server" ID="ddlPelmetSize" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="63" Text="63mm"></asp:ListItem>
                                            <asp:ListItem Value="90" Text="90mm"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>                                    
                                </div>

                                <div class="mb-3 row" runat="server">
                                    <label class="col-lg-3 col-form-label">PELMET WIDTH</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:TextBox runat="server" ID="txtPelmetWidth" TextMode="Number" CssClass="form-control" placeholder="Width ..." autocomplete="off"></asp:TextBox>
                                        <small class="form-hint">* Width</small>
                                    </div>

                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                        <a class="btn btn-primary " data-bs-toggle="offcanvas" href="#canvasInfo" role="button" aria-controls="canvasInfo" onclick="return showInfo('Pelmet Width');"><i class="bi bi-info-circle me-2"></i>Info</a>
                                    </div>
                                </div>

                                <div class="mb-5 row" runat="server" id="divReturnLength">
                                    <label class="col-lg-3 col-form-label">RETURN LENGTH</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12" runat="server" id="divReturnLeft">
                                        <asp:TextBox runat="server" ID="txtReturnLeft" TextMode="Number" CssClass="form-control" placeholder="Length ..." autocomplete="off"></asp:TextBox>
                                        <small class="form-hint">* Left</small>
                                    </div>

                                    <div class="col-lg-2 col-sm-12" runat="server" id="divReturnRight">
                                        <asp:TextBox runat="server" ID="txtReturnRight" TextMode="Number" CssClass="form-control" placeholder="Length ..." autocomplete="off"></asp:TextBox>
                                        <small class="form-hint">* Right</small>
                                    </div>

                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                        <a class="btn btn-primary " data-bs-toggle="offcanvas" href="#canvasInfo" role="button" aria-controls="canvasInfo" onclick="return showInfo('Pelmet Return');"><i class="bi bi-info-circle me-2"></i>Info</a>
                                    </div>
                                </div>

                                <div class="row">
                                    <p style="color:red;">
                                        <b><u>CUT OUTS</u></b>
                                    </p>
                                </div>

                                <div class="mb-1 row">
                                    <p style="color:red;">
                                        <b><u>TOP</u></b>
                                    </p>
                                </div>

                                <div class="mb-3 row">
                                    <label class="col-lg-3 col-form-label">LHS WIDTH - HEIGHT</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:TextBox runat="server" ID="txtTopLHSWidth" TextMode="Number" CssClass="form-control" placeholder="Width ..." autocomplete="off"></asp:TextBox>
                                        <small class="form-hint">* Width</small>
                                    </div>

                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:TextBox runat="server" ID="txtTopLHSHeigth" TextMode="Number" CssClass="form-control" placeholder="Height ..." autocomplete="off"></asp:TextBox>
                                        <small class="form-hint">* Height</small>
                                    </div>
                                </div>

                                <div class="mb-5 row">
                                    <label class="col-lg-3 col-form-label">RHS WIDTH - HEIGHT</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:TextBox runat="server" ID="txtTopRHSWidth" TextMode="Number" CssClass="form-control" placeholder="Width ..." autocomplete="off"></asp:TextBox>
                                        <small class="form-hint">* Width</small>
                                    </div>

                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:TextBox runat="server" ID="txtTopRHSHeigth" TextMode="Number" CssClass="form-control" placeholder="Height ..." autocomplete="off"></asp:TextBox>
                                        <small class="form-hint">* Height</small>
                                    </div>
                                </div>

                                <div class="mb-1 row">
                                    <p style="color:red;">
                                        <b><u>BOTTOM</u></b>
                                    </p>
                                </div>

                                <div class="mb-3 row" runat="server" id="divBottomLHS">
                                    <label class="col-lg-3 col-form-label">LHS WIDTH - HEIGHT</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:TextBox runat="server" ID="txtBottomLHSWidth" TextMode="Number" CssClass="form-control" placeholder="Width ..." autocomplete="off"></asp:TextBox>
                                        <small class="form-hint">* Width</small>
                                    </div>

                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:TextBox runat="server" ID="txtBottomLHSHeigth" CssClass="form-control" placeholder="Height ..." autocomplete="off"></asp:TextBox>
                                        <small class="form-hint">* Height</small>
                                    </div>
                                </div>

                                <div class="mb-3 row">
                                    <label class="col-lg-3 col-form-label">RHS WIDTH - HEIGHT</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:TextBox runat="server" ID="txtBottomRHSWidth" TextMode="Number" CssClass="form-control" placeholder="Width ..." autocomplete="off"></asp:TextBox>
                                        <small class="form-hint">* Width</small>
                                    </div>

                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <asp:TextBox runat="server" ID="txtBottomRHSHeigth" TextMode="Number" CssClass="form-control" placeholder="Heigth ..." autocomplete="off"></asp:TextBox>
                                        <small class="form-hint">* Height</small>
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
                    <i class="bi bi-check2-circle m-2" style="font-size: 12pt;"></i>Ok. I Got It
                </button>
            </div>
        </div>
    </div>

    <!-- my custom script -->
     <script type="text/javascript" src="/Content/dist/js/my/venetian.js"></script>
    <script type="text/javascript">

        document.addEventListener("DOMContentLoaded", function () {
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
            if (Type == 'Pelmet Return') {
                spanInfo = 'If you leave this blank, it will use the factory default, which is 67mm !';
            } else if (Type == 'Pelmet Width') {
                spanInfo = 'If you leave this blank, it will use the factory default.';
                spanInfo += '<br /><br />';
                spanInfo += 'Our Standar Pelmet Width:';
                spanInfo += '<br />';
                spanInfo += 'Reveal fit is width + 7';
                spanInfo += '<br />';
                spanInfo += 'Face fit is width + 20';
            } else if (Type == 'Quantity') {
                spanInfo = 'Please pay attention to the quantity you want to order, because the quantity you enter will be processed automatically.';
            } else if (Type == 'Control Length') {
                spanInfo = 'If you leave this blank, it will automatically follow the factory default.';
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

        <asp:Label runat="server" ID="lblCutOut_LeftTop"></asp:Label>
        <asp:Label runat="server" ID="lblCutOut_LeftBottom"></asp:Label>
        <asp:Label runat="server" ID="lblCutOut_RightTop"></asp:Label>
        <asp:Label runat="server" ID="lblCutOut_RightBottom"></asp:Label>
        
        <asp:SqlDataSource ID="sdsPage" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" InsertCommand="INSERT OrderDetails(Id, HeaderId, BlindNo, KitId, SoeKitId, PriceGroupId, Qty, Location, Mounting, Width, [Drop], ControlPosition, ControlLength, BottomHoldDown, CutOut_LeftTop, CutOut_RightTop, CutOut_LeftBottom, CutOut_RightBottom, PelmetType, PelmetWidth, PelmetSize, PelmetReturnSize, PelmetReturnSize2, LHSWidth_Top, LHSHeight_Top, RHSWidth_Top, RHSHeight_Top, LHSWidth_Bottom, LHSHeight_Bottom, RHSWidth_Bottom, RHSHeight_Bottom, Notes, Matrix, Charge, TotalMatrix, TotalCharge, MarkUp, Active) VALUES(@Id, @HeaderId, 'Blind 1', @KitId, @SoeKitId, @PriceGroupId, @Qty, @Location, @Mounting, @Width, @Drop, @ControlPosition, @ControlLength, @BottomHoldDown, @CutOut_LeftTop, @CutOut_RightTop, @CutOut_LeftBottom, @CutOut_RightBottom, @PelmetType, @PelmetWidth, @PelmetSize, @PelmetReturnSize, @PelmetReturnSize2, @LHSWidth_Top, @LHSHeight_Top, @RHSWidth_Top, @RHSHeight_Top, @LHSWidth_Bottom, @LHSHeight_Bottom, @RHSWidth_Bottom, @RHSHeight_Bottom, @Notes, 0.00, 0.00, 0.00, 0.00, @MarkUp, 1)" UpdateCommand="UPDATE OrderDetails SET BlindNo='Blind 1', KitId=@KitId, SoeKitId=@SoeKitId, PriceGroupId=@PriceGroupId, Qty=@Qty, Location=@Location, Mounting=@Mounting, Width=@Width, [Drop]=@Drop, ControlPosition=@ControlPosition, ControlLength=@ControlLength, BottomHoldDown=@BottomHoldDown, CutOut_LeftTop=@CutOut_LeftTop, CutOut_RightTop=@CutOut_RightTop, CutOut_LeftBottom=@CutOut_LeftBottom, CutOut_RightBottom=@CutOut_RightBottom, PelmetType=@PelmetType, PelmetWidth=@PelmetWidth, PelmetSize=@PelmetSize, PelmetReturnSize=@PelmetReturnSize, PelmetReturnSize2=@PelmetReturnSize, LHSWidth_Top=@LHSWidth_Top, LHSHeight_Top=@LHSHeight_Top, RHSWidth_Top=@RHSWidth_Top, RHSHeight_Top=@RHSHeight_Top, LHSWidth_Bottom=@LHSWidth_Bottom, LHSHeight_Bottom=@LHSHeight_Bottom, RHSWidth_Bottom=@RHSWidth_Bottom, RHSHeight_Bottom=@RHSHeight_Bottom, Notes=@Notes, MarkUp=@MarkUp, Active=1 WHERE Id=@Id">
            <InsertParameters>
                <asp:ControlParameter ControlID="lblItemId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblHeaderId" Name="HeaderId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblKitId" Name="KitId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblSoeKitId" Name="SoeKitId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblPriceGroupId" Name="PriceGroupId" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtQty" Name="Qty" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtLocation" Name="Location" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlMounting" Name="Mounting" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtWidth" Name="Width" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtDrop" Name="Drop" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlControl" Name="ControlPosition" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtControlLength" Name="ControlLength" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlHoldDown" Name="BottomHoldDown" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblCutOut_LeftTop" Name="CutOut_LeftTop" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblCutOut_RightTop" Name="CutOut_RightTop" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblCutOut_LeftBottom" Name="CutOut_LeftBottom" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblCutOut_RightBottom" Name="CutOut_RightBottom" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlPelmetType" Name="PelmetType" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtPelmetWidth" Name="PelmetWidth" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlPelmetSize" Name="PelmetSize" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtReturnLeft" Name="PelmetReturnSize" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtReturnRight" Name="PelmetReturnSize2" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtTopLHSWidth" Name="LHSWidth_Top" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtTopLHSHeigth" Name="LHSHeight_Top" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtTopRHSWidth" Name="RHSWidth_Top" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtTopRHSHeigth" Name="RHSHeight_Top" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtBottomLHSWidth" Name="LHSWidth_Bottom" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtBottomLHSHeigth" Name="LHSHeight_Bottom" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtBottomRHSWidth" Name="RHSWidth_Bottom" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtBottomRHSHeigth" Name="RHSHeight_Bottom" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtNotes" Name="Notes" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtMarkUp" Name="MarkUp" PropertyName="Text" />
            </InsertParameters>
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblItemId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblHeaderId" Name="HeaderId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblKitId" Name="KitId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblSoeKitId" Name="SoeKitId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblPriceGroupId" Name="PriceGroupId" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtQty" Name="Qty" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtLocation" Name="Location" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlMounting" Name="Mounting" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtWidth" Name="Width" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtDrop" Name="Drop" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlControl" Name="ControlPosition" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtControlLength" Name="ControlLength" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlHoldDown" Name="BottomHoldDown" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="lblCutOut_LeftTop" Name="CutOut_LeftTop" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblCutOut_RightTop" Name="CutOut_RightTop" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblCutOut_LeftBottom" Name="CutOut_LeftBottom" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblCutOut_RightBottom" Name="CutOut_RightBottom" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlPelmetType" Name="PelmetType" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtPelmetWidth" Name="PelmetWidth" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlPelmetSize" Name="PelmetSize" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtReturnLeft" Name="PelmetReturnSize" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtReturnRight" Name="PelmetReturnSize2" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtTopLHSWidth" Name="LHSWidth_Top" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtTopLHSHeigth" Name="LHSHeight_Top" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtTopRHSWidth" Name="RHSWidth_Top" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtTopRHSHeigth" Name="RHSHeight_Top" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtBottomLHSWidth" Name="LHSWidth_Bottom" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtBottomLHSHeigth" Name="LHSHeight_Bottom" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtBottomRHSWidth" Name="RHSWidth_Bottom" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtBottomRHSHeigth" Name="RHSHeight_Bottom" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtNotes" Name="Notes" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtMarkUp" Name="MarkUp" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>