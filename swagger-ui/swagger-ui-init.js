
window.onload = function() {
  // Build a system
  var url = window.location.search.match(/url=([^&]+)/);
  if (url && url.length > 1) {
    url = decodeURIComponent(url[1]);
  } else {
    url = window.location.origin;
  }
  var options = {
  "swaggerDoc": {
    "openapi": "3.0.0",
    "info": {
      "version": "1.0.0",
      "title": "CLG Images CDN API",
      "description": "",
      "license": {
        "name": "MIT",
        "url": "https://opensource.org/licenses/MIT"
      }
    },
    "tags": [
      {
        "name": "All Games",
        "description": "All Games"
      },
      {
        "name": "Lobby Games",
        "description": "Lobby Games"
      },
      {
        "name": "Headers",
        "description": "Headers"
      }
    ],
    "servers": [
      {
        "url": "http://localhost/cdn",
        "description": "Development server"
      },
      {
        "url": "https://imgtest.playliga.com",
        "description": "Test server"
      },
      {
        "url": "https://border-px1-api.xyz",
        "description": "Production server"
      }
    ],
    "paths": {
      "/allgames/create": {
        "post": {
          "tags": [ "All Games" ],
          "summary": "upload an ALL GAMEs image ",
          "requestBody": {
            "content": {
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/allgamesRequestBody"
                }
              }
            },
            "required": true
          },
          "responses": {
            "200": {
              "description": "Send request successfully",
              "content": {
                "application/json": {
                  "schema": {
                    "$ref": "#/components/schemas/bositeResponse"
                  }
                }
              }
            }
          }
        }
      },
      "/allgames/update": {
        "put": {
          "tags": [ "All Games" ],
          "summary": "update an ALL GAMEs image ",
          "requestBody": {
            "content": {
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/allgamesRequestBody"
                }
              }
            },
            "required": true
          },
          "responses": {
            "200": {
              "description": "Send request successfully",
              "content": {
                "application/json": {
                  "schema": {
                    "$ref": "#/components/schemas/bositeResponse"
                  }
                }
              }
            }
          }
        }
      },
      "/allgames/delete": {
        "delete": {
          "tags": [ "All Games" ],
          "summary": "delete an ALL GAMEs image ",
          "requestBody": {
            "content": {
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/allgamesRequestBody"
                }
              }
            },
            "required": true
          },
          "responses": {
            "200": {
              "description": "Send request successfully",
              "content": {
                "application/json": {
                  "schema": {
                    "$ref": "#/components/schemas/bositeResponse"
                  }
                }
              }
            }
          }
        }
      },
      "/lobbygames/create": {
        "post": {
          "tags": [ "Lobby Games" ],
          "summary": "upload a lobby game image ",
          "requestBody": {
            "content": {
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/lobbygamesRequestBody"
                }
              }
            },
            "required": true
          },
          "responses": {
            "200": {
              "description": "Send request successfully",
              "content": {
                "application/json": {
                  "schema": {
                    "$ref": "#/components/schemas/bositeResponse"
                  }
                }
              }
            }
          }
        }
      },
      "/lobbygames/update": {
        "put": {
          "tags": [ "Lobby Games" ],
          "summary": "update a lobby game image ",
          "requestBody": {
            "content": {
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/lobbygamesRequestBody"
                }
              }
            },
            "required": true
          },
          "responses": {
            "200": {
              "description": "Send request successfully",
              "content": {
                "application/json": {
                  "schema": {
                    "$ref": "#/components/schemas/bositeResponse"
                  }
                }
              }
            }
          }
        }
      },
      "/lobbygames/delete": {
        "delete": {
          "tags": [ "Lobby Games" ],
          "summary": "delete a lobby game image ",
          "requestBody": {
            "content": {
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/lobbygamesRequestBody"
                }
              }
            },
            "required": true
          },
          "responses": {
            "200": {
              "description": "Send request successfully",
              "content": {
                "application/json": {
                  "schema": {
                    "$ref": "#/components/schemas/bositeResponse"
                  }
                }
              }
            }
          }
        }
      }
    },
    "components": {
      "schemas": {
        "allgamesRequestBody": {
          "required": true,
          "properties": {
            "gameListID": {
              "type": "string"
            },
            "imageType": {
              "type": "string"
            },
            "strBase64": {
              "type": "string"
            }
          },
          "example": {
            "gameListID": "100789",
            "imageType": "png",
            "strBase64": "iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAApgAAAKYB3X3/OAAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAANCSURBVEiJtZZPbBtFFMZ/M7ubXdtdb1xSFyeilBapySVU8h8OoFaooFSqiihIVIpQBKci6KEg9Q6H9kovIHoCIVQJJCKE1ENFjnAgcaSGC6rEnxBwA04Tx43t2FnvDAfjkNibxgHxnWb2e/u992bee7tCa00YFsffekFY+nUzFtjW0LrvjRXrCDIAaPLlW0nHL0SsZtVoaF98mLrx3pdhOqLtYPHChahZcYYO7KvPFxvRl5XPp1sN3adWiD1ZAqD6XYK1b/dvE5IWryTt2udLFedwc1+9kLp+vbbpoDh+6TklxBeAi9TL0taeWpdmZzQDry0AcO+jQ12RyohqqoYoo8RDwJrU+qXkjWtfi8Xxt58BdQuwQs9qC/afLwCw8tnQbqYAPsgxE1S6F3EAIXux2oQFKm0ihMsOF71dHYx+f3NND68ghCu1YIoePPQN1pGRABkJ6Bus96CutRZMydTl+TvuiRW1m3n0eDl0vRPcEysqdXn+jsQPsrHMquGeXEaY4Yk4wxWcY5V/9scqOMOVUFthatyTy8QyqwZ+kDURKoMWxNKr2EeqVKcTNOajqKoBgOE28U4tdQl5p5bwCw7BWquaZSzAPlwjlithJtp3pTImSqQRrb2Z8PHGigD4RZuNX6JYj6wj7O4TFLbCO/Mn/m8R+h6rYSUb3ekokRY6f/YukArN979jcW+V/S8g0eT/N3VN3kTqWbQ428m9/8k0P/1aIhF36PccEl6EhOcAUCrXKZXXWS3XKd2vc/TRBG9O5ELC17MmWubD2nKhUKZa26Ba2+D3P+4/MNCFwg59oWVeYhkzgN/JDR8deKBoD7Y+ljEjGZ0sosXVTvbc6RHirr2reNy1OXd6pJsQ+gqjk8VWFYmHrwBzW/n+uMPFiRwHB2I7ih8ciHFxIkd/3Omk5tCDV1t+2nNu5sxxpDFNx+huNhVT3/zMDz8usXC3ddaHBj1GHj/As08fwTS7Kt1HBTmyN29vdwAw+/wbwLVOJ3uAD1wi/dUH7Qei66PfyuRj4Ik9is+hglfbkbfR3cnZm7chlUWLdwmprtCohX4HUtlOcQjLYCu+fzGJH2QRKvP3UNz8bWk1qMxjGTOMThZ3kvgLI5AzFfo379UAAAAASUVORK5CYII="
          }
        },
        "lobbygamesRequestBody": {
          "required": true,
          "properties": {
            "CTId": {
              "type": "string"
            },
            "GameLobbyId": {
              "type": "string"
            },
            "GameCode": {
              "type": "string"
            },
            "ImageType": {
              "type": "string"
            },
            "strBase64": {
              "type": "string"
            }
          },
          "example": {
            "CTId": "890710",
            "GameLobbyId": "1234",
            "GameCode": "4321",
            "ImageType": "png",
            "strBase64": "iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAApgAAAKYB3X3/OAAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAANCSURBVEiJtZZPbBtFFMZ/M7ubXdtdb1xSFyeilBapySVU8h8OoFaooFSqiihIVIpQBKci6KEg9Q6H9kovIHoCIVQJJCKE1ENFjnAgcaSGC6rEnxBwA04Tx43t2FnvDAfjkNibxgHxnWb2e/u992bee7tCa00YFsffekFY+nUzFtjW0LrvjRXrCDIAaPLlW0nHL0SsZtVoaF98mLrx3pdhOqLtYPHChahZcYYO7KvPFxvRl5XPp1sN3adWiD1ZAqD6XYK1b/dvE5IWryTt2udLFedwc1+9kLp+vbbpoDh+6TklxBeAi9TL0taeWpdmZzQDry0AcO+jQ12RyohqqoYoo8RDwJrU+qXkjWtfi8Xxt58BdQuwQs9qC/afLwCw8tnQbqYAPsgxE1S6F3EAIXux2oQFKm0ihMsOF71dHYx+f3NND68ghCu1YIoePPQN1pGRABkJ6Bus96CutRZMydTl+TvuiRW1m3n0eDl0vRPcEysqdXn+jsQPsrHMquGeXEaY4Yk4wxWcY5V/9scqOMOVUFthatyTy8QyqwZ+kDURKoMWxNKr2EeqVKcTNOajqKoBgOE28U4tdQl5p5bwCw7BWquaZSzAPlwjlithJtp3pTImSqQRrb2Z8PHGigD4RZuNX6JYj6wj7O4TFLbCO/Mn/m8R+h6rYSUb3ekokRY6f/YukArN979jcW+V/S8g0eT/N3VN3kTqWbQ428m9/8k0P/1aIhF36PccEl6EhOcAUCrXKZXXWS3XKd2vc/TRBG9O5ELC17MmWubD2nKhUKZa26Ba2+D3P+4/MNCFwg59oWVeYhkzgN/JDR8deKBoD7Y+ljEjGZ0sosXVTvbc6RHirr2reNy1OXd6pJsQ+gqjk8VWFYmHrwBzW/n+uMPFiRwHB2I7ih8ciHFxIkd/3Omk5tCDV1t+2nNu5sxxpDFNx+huNhVT3/zMDz8usXC3ddaHBj1GHj/As08fwTS7Kt1HBTmyN29vdwAw+/wbwLVOJ3uAD1wi/dUH7Qei66PfyuRj4Ik9is+hglfbkbfR3cnZm7chlUWLdwmprtCohX4HUtlOcQjLYCu+fzGJH2QRKvP3UNz8bWk1qMxjGTOMThZ3kvgLI5AzFfo379UAAAAASUVORK5CYII="
          }
        },
        "bositeResponse": {
          "required": [ "success" ],
          "properties": {
            "success": {
              "type": "boolean"
            },
            "message": {
              "type": "string"
            }
          }
        },
        "securitySchemes": {
          "bearerAuth": {
            "type": "apiKey",
            "description": "Type into the textbox: Bearer {your token}.",
            "name": "Authorization",
            "in": "header"
          }
        }
      }
    },
    "security": [
      {
        "bearerAuth": []
      }
    ]
  },  
  "customOptions": {}
};
  url = options.swaggerUrl || url
  var urls = options.swaggerUrls
  var customOptions = options.customOptions
  var spec1 = options.swaggerDoc
  var swaggerOptions = {
    spec: spec1,
    url: url,
    urls: urls,
    dom_id: '#swagger-ui',
    deepLinking: true,
    presets: [
      SwaggerUIBundle.presets.apis,
      SwaggerUIStandalonePreset
    ],
    plugins: [
      SwaggerUIBundle.plugins.DownloadUrl
    ],
    layout: "StandaloneLayout"
  }
  for (var attrname in customOptions) {
    swaggerOptions[attrname] = customOptions[attrname];
  }
  var ui = SwaggerUIBundle(swaggerOptions)

  if (customOptions.oauth) {
    ui.initOAuth(customOptions.oauth)
  }

  if (customOptions.authAction) {
    ui.authActions.authorize(customOptions.authAction)
  }

  window.ui = ui
}
