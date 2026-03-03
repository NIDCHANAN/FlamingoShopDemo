function loginWithLine() {

    const channelId = "2009182005";
    const redirectUri = encodeURIComponent("https://flamingoflower.digital/Home/LineCallback");
    const state = "abc123"; // กัน CSRF

    const url =
        `https://access.line.me/oauth2/v2.1/authorize?response_type=code` +
        `&client_id=${channelId}` +
        `&redirect_uri=${redirectUri}` +
        `&state=${state}` +
        `&scope=profile%20openid%20email`;

    window.location.href = url;
}