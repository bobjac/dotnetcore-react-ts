export const server = 'http://localhost:5000';

export const webAPIUrl = `${server}/api`;

export const authSettings = {
  domain: 'bobjac.auth0.com',
  client_id: 'W9tVSOmiQX2qN1oR6RkTpd0jjN5YZHbL',
  redirect_uri: window.location.origin + '/signin-callback',
  scope: 'openid profile QandAAPI email',
  audience: 'https://qanda',
};