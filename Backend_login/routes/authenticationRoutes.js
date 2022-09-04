const mongoose = require('mongoose');
const Account=mongoose.model('accounts');

module.exports=app => {

    //Routes
    app.post('/account/login', async (req, res) => {

        var response= {} ;

        const { rUsername, rPassword }= req.body;
        if(rUsername==null || rPassword==null){
            response.code=1;
            response.msg="Invalid crededential"
            res.send(response);
            return;
        }
    
        var userAccount= await Account.findOne({username:rUsername});
        if(userAccount != null){
            if(rPassword == userAccount.password){
                userAccount.lastAuthentication=Date.now();
                await userAccount.save();

                response.code=0;
                response.msg="Account found";
                response.data= userAccount;
                res.send(response);

                console.log("Retriving account...")
                res.send(userAccount);

                return;
            }
        }

        response.code=1;
        response.msg="Invalid crededential";
        res.send(response);
        return;


    });

    app.post('/account/create', async (req, res) => {

        var response= {} ;

        const { rUsername, rPassword }= req.body;
        if(rUsername==null || rPassword==null){
            response.code=1;
            response.msg="Invalid crededential"
            res.send(response);
            return;
        }
    
        var userAccount= await Account.findOne({username:rUsername});
        if(userAccount == null){
            console.log("Create new account...");

            var newAccount= new Account({
                username: rUsername,
                password: rPassword,
  
                lastAuthentication : Date.now()
            });
            await newAccount.save();

            response.code=0;
            response.msg="Account found";
            response.data= userAccount;
            res.send(response);
            return;
        } else{
            response.code=2;
            response.msg="Username is already taken"
            res.send(response);
        }

        return;


    });

}
