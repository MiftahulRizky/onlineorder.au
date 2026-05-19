<%@ Page Title="Report Product" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.vb" Inherits="ReportProduct_Default" Debug="true" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">
                    <span id="pageAction">report</span>
                    </div>
                    <h2 class="page-title" id="pageTitle">Product</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl" id="pageContent">
            <div class="row mb-3">
                <div class="col-lg-4 col-md-4 col-sm-4 mx-auto">
                    <div class="card" id="cardFind">
                        <div class="card-header">
                            <h3 class="card-title">Find</h3>
                        </div>
                        <form action="javascript:void(0)" method="post" id="formSubmit">
                            <div class="card-body row">
                            <div class="col-lg-12 mb-3">
                                    <label for="findby" class="form-label">Find By</label>
                                    <select class="form-select" id="findby" name="findby">
                                    </select>
                            </div>
                            <div class="col-lg-12 mb-2">
                                    <label for="fined" class="form-label text-capitalize" id="finedLabel">Fined</label>
                                    <select class="form-select" id="fined" name="fined">
                                    </select>
                            </div>

                            <div class="col-lg-6 mb-2">
                                    <label for="fromdate" class="form-label ">From</label>
                                    <input type="date" class="form-control" id="fromdate" name="fromdate">
                            </div>

                            <div class="col-lg-6 mb-2">
                                    <label for="todate" class="form-label">To</label>
                                    <input type="date" class="form-control" id="todate" name="todate">
                            </div>
                            </div>
                            <div class="card-footer text-center">
                                <button type="submit" class="btn btn-primary" id="btnFind">Show</button>
                                <button type="button" class="btn btn-danger ms-2" id="btnReset">Reset</button>
                            </div>
                        </form>
                    </div>
                </div>

                <div class="col-lg-8 col-md-8 col-sm-8" hidden>
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Result</h3>
                        </div>
                        <div class="card-body">
                            <div id="table-default">
                                <table class="table table-vcenter card-table datatable" id="reportServerSide" width="100%">
                                    <thead class="fs-3 fw-bold">
                                        <tr>
                                            <th class="text-center">#</th>
                                            <th class="h3 text-center">Cistomers</th>
                                            <th class="h3">Products</th>
                                        </tr>
                                    </thead>
                                    <tbody></tbody>
                                </table>

                            </div>
                        </div>
                        <div class="card-footer text-center">
                            <button type="button" class="btn btn-success" id="btnGeneratePDF">Generate PDF Report</button>
                        </div>
                    </div>
                </div>
            </div>      
        </div>
    </div>

    <script type="text/javascript">
        let ROLENAME = '<%= Session("RoleName") %>';
        let URIMETHOD = "/Methods/ReportProductMethod.aspx";
    </script>

    <script type="text/javascript" src="/Scripts/ReportProduct.js?<%= DateTime.Now.Ticks %>"></script>
</asp:Content>

