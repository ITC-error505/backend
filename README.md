# Backend for Pizza Parlor's mobile website

## About
The backend endpoints for Information Technology Competition. These are paired together with the frontend. 


## API reference

POST	account/login   
body: String username, String password   
returns (success 200 and JWT with id field or 400 fail)   

POST account/register   
body: String username, String password   
returns (success 200 or 400 fail)   

POST /score/post   
body: int score, int gameId   
bearerToken  NameIdentifier: string accountId (tokenized string)   
returns (success 200 or 400 fail)   

GET /score/leaderboard   
query string: int gameId   
returns a list of {string username, int score, int rank}   

GET /score/highScore   
query string: int gameId   
bearerToken  NameIdentifier: string accountId (tokennized string)   
return {int highScore, int rank}   

GET /games/all   
query string:   
return a list of {int id, string name}   



