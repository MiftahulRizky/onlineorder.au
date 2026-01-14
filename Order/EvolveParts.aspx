<%@ Page Language="VB" AutoEventWireup="false" CodeFile="EvolveParts.aspx.vb" Inherits="Order_EvolveParts" MasterPageFile="~/Site.Master" Title="Evolve Parts" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .datagridTitle{ font-size:14px; }
        .datagridContent { font-size:16px; }
    </style>

    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">
                        <span>ORDER</span>
                    </div>
                    <h2 class="page-title" id="pageTitle"></h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container container-slim py-4" id="divLoader">
            <div class="text-center">
                <div class="row mb-xxl-7">
                    <br />
                </div>
                <div class="row mb-xxl-8">
                    <a href="." class="navbar-brand navbar-brand-autodark"><img runat="server" src="~/Content/static/ShutterLogo.png"></a>
                </div>
                <div class="text-secondary mb-3">PREPARING DATA</div>
                <div class="progress progress-sm">
                    <div class="progress-bar progress-bar-indeterminate"></div>
                </div>
            </div>
        </div>

        <div class="container-xl" id="divOrder" style="display:none;">
            <div class="row mb-3">
                <div class="col-lg-8 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-body">
                            <div class="datagrid">
                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">Order #</div>
                                    <div class="datagrid-content datagridContent">
                                        <span id="orderid"></span>
                                    </div>
                                </div>
                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">Order Number</div>
                                    <div class="datagrid-content datagridContent">
                                        <span id="orderno"></span>
                                    </div>
                                </div>
                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">Customer Name</div>
                                    <div class="datagrid-content datagridContent">
                                        <span id="ordername"></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-lg-8 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title" id="cardTitle"></h3>
                        </div>

                        <div class="card-body">
                            <div class="mb-2 row">
                                <label class="col-lg-3 col-form-label required">PRODUCT</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <select class="form-select" id="blindtype" style="font-weight:bold;"></select>
                                </div>
                            </div>

                            <div class="mb-2 row" style="display:none;">
                                <label class="col-lg-3 col-form-label required">PRODUCT</label>
                                <div class="col-lg-5 col-md-12 col-sm-12">
                                    <select class="form-select" id="colourtype" style="font-weight:bold;"></select>
                                </div>
                            </div>

                            <div id="divDetail">
                                <hr />

                                <div class="mb-5 row">
                                    <label class="col-lg-3 col-form-label required">QUANTITY</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <input type="number" id="qty" class="form-control" autocomplete="off" placeholder="Quantity" value="1" />
                                    </div> 
                                </div>

                                <div class="mb-2 row">
                                    <label class="col-lg-3 col-form-label required">CATEGORY</label>
                                    <div class="col-lg-6 col-md-12 col-sm-12">
                                        <select class="form-select" id="category">
                                            <option value=""></option>
                                            <option value="Louvres">LOUVRES</option>
                                            <option value="Framing | Hinged">FRAMING | HINGED</option>
                                            <option value="Framing | Bi-fold or Sliding">FRAMING | BI-FOLD OR SLIDING</option>
                                            <option value="Framing | Fixed">FRAMING | FIXED</option>
                                            <option value="Posts">POSTS</option>
                                            <option value="Extrusion">EXTRUSION</option>
                                            <option value="Hinges">HINGES</option>
                                            <option value="Catches">CATCHES</option>
                                            <option value="Track Hardware">TRACK HARDWARE</option>
                                            <option value="Misc">MISC</option>
                                        </select>
                                    </div>
                                </div>

                                <div class="mb-2 row">
                                    <label class="col-lg-3 col-form-label required">COMPONENT</label>
                                    <div class="col-lg-7 col-md-12 col-sm-12">
                                        <select class="form-select" id="component"></select>
                                    </div>
                                </div>

                                <div class="mb-2 row" id="divColour">
                                    <label class="col-lg-3 col-form-label required">COLOUR</label>
                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                        <select class="form-select" id="colour"></select>
                                    </div>
                                </div>

                                <div class="mb-2 row" id="divLength">
                                    <label class="col-lg-3 col-form-label required">LENGTH</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <div class="input-group">
                                            <input type="number" id="length" class="form-control" autocomplete="off" placeholder="Length" />
                                            <span class="input-group-text">mm</span>
                                        </div>
                                    </div>
                                </div>

                                <div class="mb-2 mt-5 row">
                                    <label class="col-lg-3 col-form-label">SPECIAL INFORMATION</label>
                                    <div class="col-lg-8 col-md-12 col-sm-12">
                                        <textarea class="form-control" id="notes" rows="6" placeholder="Your notes ..." style="resize:none;"></textarea>
                                        <span class="form-label-description" id="notescount">0/1000</span>
                                    </div>
                                </div>

                                <div class="mb-5 row">
                                    <label class="col-lg-3 col-form-label">MARK UP</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <div class="input-group">
                                            <input type="number" id="markup" class="form-control" autocomplete="off" placeholder="Mark Up ..." />
                                            <span class="input-group-text">%</span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="card-footer text-center">
                            <a href="javascript:void(0);" id="submit" class="btn btn-primary">Submit</a>
                            <a href="javascript:void(0);" id="cancel" class="btn btn-danger">Cancel</a>
                        </div>
                    </div>
                </div>

                <div class="col-lg-4 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Notes</h3>
                        </div>

                        <div class="card-body">
                            <div class="markdown">
                                <ul></ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalSuccess" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-status bg-success"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" class="icon mb-2 text-green icon-lg" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M12 12m-9 0a9 9 0 1 0 18 0a9 9 0 1 0 -18 0" /><path d="M9 12l2 2l4 -4" /></svg>
                    <h3>Successfully</h3>
                    <div class="text-secondary">Your order has been successfully saved</div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="javascript:void(0);" id="vieworder" class="btn btn-success w-100" data-bs-dismiss="modal">View Order</a></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalError" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-danger"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" class="icon mb-2 text-danger icon-lg" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M10.24 3.957l-8.422 14.06a1.989 1.989 0 0 0 1.7 2.983h16.845a1.989 1.989 0 0 0 1.7 -2.983l-8.423 -14.06a1.989 1.989 0 0 0 -3.4 0z" /><path d="M12 9v4" /><path d="M12 17h.01" /></svg>
                    <h3>Important Message</h3>
                    <div class="text-secondary" id="errorMsg"></div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn btn-danger w-100" data-bs-dismiss="modal">CLOSE</a></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        let designIdOri = 'DAA2D2CD-B5B6-49FA-A9F8-C65A609440BE';
        let headerId = '<%= Session("headerId") %>';
        let itemAction = '<%= Session("itemAction") %>';
        let designId = '<%= Session("designId") %>';
        let itemId = '<%= Session("itemId") %>';
        let loginId = '<%= Session("LoginId") %>';
    </script>
    <script src="../Scripts/Order/EvolveParts.js?v=<%= DateTime.Now.Ticks %>"></script>
</asp:Content>